using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance { get; private set; }

    [SerializeField] private AudioClip defaultMusicClip;
    [SerializeField] private AudioMixerGroup musicMixer;
    AudioSource musicSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = defaultMusicClip;
        musicSource.loop = true;
        musicSource.outputAudioMixerGroup = musicMixer;
        musicSource.playOnAwake = false;
        musicSource.volume = 0.5f;
    }

    public void PlayMusic()
    {
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
