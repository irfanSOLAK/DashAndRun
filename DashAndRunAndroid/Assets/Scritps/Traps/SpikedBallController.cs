using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikedBallController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform allParent;
    [SerializeField] private GameObject chain;                     // Zincir segment prefab'ý
    [SerializeField] private Transform chainStartPoint;           // Zincirin baþlangýç noktasý
    [SerializeField] private Transform chainEndPoint;            // Zincirin bitiþ noktasý
    [SerializeField] private int numberOfChains = 10;           // Zincir segmentlerinin sayýsý
    [SerializeField] private float swingAngle;  // Sallanma mesafesi
    [SerializeField] private float swingSpeed;  // Sallanma hýzý


    private GameObject[] chains;
    private float currentAngle;
    private float timer;
    private AudioClip clip;
    private bool hasPlayedSound; // Sesin çalýnýp çalýnmadýðýný takip eden deðiþken
    private float previousAngle; // Önceki açýyý takip eder

    private void Awake()
    {
        GameBehaviour.Instance.Audio.AddAudioSource(audioSource);
    }

    void Start()
    {
        clip = Resources.Load<AudioClip>("Sounds/SpikedBall");
        chain.transform.position = chainStartPoint.position;
        CreateChainSegments();
        currentAngle = allParent.transform.rotation.z;
        hasPlayedSound = false;
        previousAngle = 0;
    }

    void CreateChainSegments()
    {
        chains = new GameObject[numberOfChains];
        float totalDistance = Vector3.Distance(chainStartPoint.position, chainEndPoint.position);
        float chainLength = totalDistance / (numberOfChains - 1);
        Vector3 direction = (chainEndPoint.position - chainStartPoint.position).normalized;

        chains[0] = chain;
        chains[0].transform.position = chainStartPoint.position;
        chains[0].transform.parent = allParent;

        for (int i = 1; i < numberOfChains; i++)
        {
            Vector3 position = chainStartPoint.position + i * chainLength * direction;
            GameObject chainSegment = Instantiate(chain, position, Quaternion.identity);
            chainSegment.transform.parent = allParent;
            chains[i] = chainSegment;
        }
    }
    void Update()
    {
        float angle = Mathf.Sin(timer) * swingAngle;
        SwingSpikedBall(angle);
        UpdateSwingSound(angle);
    }

    void SwingSpikedBall(float angle)
    {
        timer += Time.deltaTime * swingSpeed;
        allParent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + currentAngle));
    }

    private void UpdateSwingSound(float angle)
    {
        // Sesin çalýnmasý için geri dönüþü kontrol et
        if (Mathf.Abs(angle) >= swingAngle * 0.9f)
        {
            hasPlayedSound = false; // Maksimum açýya ulaþýldýðýnda sesin tekrar çalabilmesi için flag'i sýfýrla
        }

        // Ses çalma kontrolü: Geri dönüþ sýrasýnda ve ses çalmadýysa
        if (Mathf.Abs(angle) < swingAngle * 0.9f && Mathf.Abs(previousAngle) >= swingAngle * 0.9f && !hasPlayedSound)
        {
            PlaySound();
            hasPlayedSound = true; // Ses çaldýktan sonra flag'i güncelle
        }

        // Geçmiþ açýyý güncelle
        previousAngle = angle;
    }
    private void PlaySound()
    {
        audioSource.PlayOneShot(clip);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameBehaviour.Instance.Notifications.PostNotification(HealtSystemManager.Hit);
        }
    }
}
