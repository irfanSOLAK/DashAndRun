using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikedBallController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform allParent;
    [SerializeField] private GameObject chain;                     // Zincir segment prefab'�
    [SerializeField] private Transform chainStartPoint;           // Zincirin ba�lang�� noktas�
    [SerializeField] private Transform chainEndPoint;            // Zincirin biti� noktas�
    [SerializeField] private int numberOfChains = 10;           // Zincir segmentlerinin say�s�
    [SerializeField] private float swingAngle;  // Sallanma mesafesi
    [SerializeField] private float swingSpeed;  // Sallanma h�z�


    private GameObject[] chains;
    private float currentAngle;
    private float timer;
    private AudioClip clip;
    private bool hasPlayedSound; // Sesin �al�n�p �al�nmad���n� takip eden de�i�ken
    private float previousAngle; // �nceki a��y� takip eder

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
        // Sesin �al�nmas� i�in geri d�n��� kontrol et
        if (Mathf.Abs(angle) >= swingAngle * 0.9f)
        {
            hasPlayedSound = false; // Maksimum a��ya ula��ld���nda sesin tekrar �alabilmesi i�in flag'i s�f�rla
        }

        // Ses �alma kontrol�: Geri d�n�� s�ras�nda ve ses �almad�ysa
        if (Mathf.Abs(angle) < swingAngle * 0.9f && Mathf.Abs(previousAngle) >= swingAngle * 0.9f && !hasPlayedSound)
        {
            PlaySound();
            hasPlayedSound = true; // Ses �ald�ktan sonra flag'i g�ncelle
        }

        // Ge�mi� a��y� g�ncelle
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
