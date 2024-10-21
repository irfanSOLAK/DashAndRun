using System.Collections;
using UnityEngine;

public class ChainSawController : MonoBehaviour
{
    [SerializeField] private GameObject saw;                         // Testere objesi
    [SerializeField] private GameObject chain;                      // Zincir segment prefab'ý
    [SerializeField] private Transform chainStartPoint;            // Zincirin baþlangýç noktasý
    [SerializeField] private Transform chainEndPoint;             // Zincirin bitiþ noktasý
    [SerializeField] private AudioSource audioSource;            // Zincirin bitiþ noktasý
    [SerializeField] private int numberOfChains = 10;           // Zincir segmentlerinin sayýsý
    [SerializeField] private float movementSpeed = 1.3f;       // Testerenin zincir boyunca hareket hýzý
    [SerializeField] private float startPercentage;

    private GameObject[] chains;
    private float movementTimer;

    private void Awake()
    {
        GameBehaviour.Instance.Audio.AddAudioSource(audioSource);
    }

    void Start()
    {
        float totalDistance = Vector3.Distance(chainStartPoint.position, chainEndPoint.position);
        float sawStartPosition = Vector3.Distance(chainStartPoint.position, saw.transform.position);
        startPercentage = sawStartPosition / totalDistance;

        chain.transform.position = chainStartPoint.position;

        CreateChainSegments();
    }

    void CreateChainSegments()
    {
        chains = new GameObject[numberOfChains];
        float totalDistance = Vector3.Distance(chainStartPoint.position, chainEndPoint.position);
        float chainLength = totalDistance / (numberOfChains - 1);
        Vector3 direction = (chainEndPoint.position - chainStartPoint.position).normalized;

        chains[0] = chain;
        chains[0].transform.position = chainStartPoint.position;
        chains[0].transform.parent = transform;

        for (int i = 1; i < numberOfChains; i++)
        {
            Vector3 position = chainStartPoint.position + i * chainLength * direction;
            GameObject chainSegment = Instantiate(chain, position, Quaternion.identity);
            chainSegment.transform.parent = transform;
            chains[i] = chainSegment;
        }
    }

    void Update()
    {
        movementTimer += movementSpeed * Time.deltaTime;
        float movePercentage = Mathf.PingPong(movementTimer+startPercentage, 1f);

        if (chains.Length > 0)
        {
            Vector3 startPosition = chains[0].transform.position;
            Vector3 endPosition = chains[chains.Length - 1].transform.position;
            saw.transform.position = Vector3.Lerp(startPosition, endPosition, movePercentage); // Testerenin konumunu hesapla
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            GameBehaviour.Instance.Notifications.PostNotification(HealtSystemManager.Hit);
        }
    }
}
