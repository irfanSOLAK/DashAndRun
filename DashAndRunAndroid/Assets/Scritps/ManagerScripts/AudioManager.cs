using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour, IManagerModule
{
    [SerializeField] List<AudioSource> allAudioSources = new List<AudioSource>();
    [SerializeField] AudioSource audioSource;

    const string backgroundMusicPath = "Sounds/BackgroundMusic";
    const string muteKey = "AudioMute";

    public string MuteKey
    {
        get { return muteKey; }
    }

    public int ManagerExecutionOrder => 30;
    public void OnModuleAwake()
    {
        SceneManager.sceneUnloaded += OnSceneChange;
        audioSource = GetAudioSource();
        AddAudioSource(audioSource);
        PlayBackgroundMusic();
    }

    private AudioSource GetAudioSource()
    {
        if (GetComponent<AudioSource>())
        {
            return GetComponent<AudioSource>();
        }

        return gameObject.AddComponent<AudioSource>();
    }

    public void AddAudioSource(AudioSource source)
    {
        allAudioSources.Add(source);
        bool isMuted = PlayerPrefs.GetInt(muteKey, 0) == 1;
        source.mute = isMuted;
    }

    private void PlayBackgroundMusic()
    {
        audioSource.clip = Resources.Load<AudioClip>(backgroundMusicPath); // Müzik parçasýný ata
        audioSource.loop = true; // Döngüye al
        audioSource.Play(); // Müziði çalmaya baþla
    }

    public float PlaySound(string soundName)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Sounds/{soundName}");
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            return clip.length;
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found!");
            return 0;
        }
    }

    public void ToggleMute()
    {
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.mute = !audioSource.mute;
        }
        PlayerPrefs.SetInt(muteKey, audioSource.mute ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnSceneChange(Scene current)
    {
        Debug.Log("asd");
        ClearAudioList();
    }

    public void ClearAudioList()
    {
        allAudioSources.Clear();
        AddAudioSource(audioSource);
    }
}
