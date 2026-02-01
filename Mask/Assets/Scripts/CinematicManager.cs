using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class CinematicManager : MonoBehaviour
{
    private static CinematicManager _instance;

    public static CinematicManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("Subtitles")]
    [SerializeField] private GameObject subtitles;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private AudioObject[] radioScriptAudioClips;
    private AudioSource audioSource;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;

    [Header("Settings")]
    [SerializeField] AudioMixerGroup soundEffectsMixerGroup;
    [SerializeField] AudioMixerGroup musicMixerGroup;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = soundEffectsMixerGroup;
        musicSource.outputAudioMixerGroup = musicMixerGroup;
        Say();
    }

    public void SetSubtitle(string text)
    {
        subtitleText.text = text;
    }

    public void ClearSubtitle()
    {
        subtitleText.text = "";
    }

    public void Say()
    {
        StartCoroutine(SayCoroutine());
    }

    private IEnumerator SayCoroutine()
    {
        foreach (AudioObject clip in radioScriptAudioClips)
        {
            StartCoroutine(PlayAudioWithSubtitles(clip));
            yield return new WaitForSeconds(clip.audioClip.length);
        }

        subtitles.SetActive(false);
        // Play music after the radio script
        audioSource.clip = musicSource.clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    private IEnumerator PlayAudioWithSubtitles(AudioObject clip)
    {
        audioSource.PlayOneShot(clip.audioClip);
        SetSubtitle(clip.subtitle);
        yield return new WaitForSeconds(clip.audioClip.length);
        ClearSubtitle();
    }

}
