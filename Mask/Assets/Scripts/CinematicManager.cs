using System.Collections;
using TMPro;
using UnityEngine;

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
    }

    private IEnumerator PlayAudioWithSubtitles(AudioObject clip)
    {
        audioSource.PlayOneShot(clip.audioClip);
        SetSubtitle(clip.subtitle);
        yield return new WaitForSeconds(clip.audioClip.length);
        ClearSubtitle();
    }

}
