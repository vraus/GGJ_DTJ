using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] public AudioMixer audioMixer;

    private static SoundMixerManager instance;

    private void Awake()
    {
        // Implement singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved settings
        LoadAudioSettings();
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("masterVolume", volume);
        PlayerPrefs.SetFloat("masterVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
        PlayerPrefs.SetFloat("musicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("sfxVolume", volume);
        PlayerPrefs.SetFloat("sfxVolume", volume);
        PlayerPrefs.Save();
    }

    private void LoadAudioSettings()
    {
        float masterVol = PlayerPrefs.GetFloat("masterVolume", 0f);
        float musicVol = PlayerPrefs.GetFloat("musicVolume", 0f);
        float sfxVol = PlayerPrefs.GetFloat("sfxVolume", 0f);

        audioMixer.SetFloat("masterVolume", masterVol);
        audioMixer.SetFloat("musicVolume", musicVol);
        audioMixer.SetFloat("sfxVolume", sfxVol);
    }
}
