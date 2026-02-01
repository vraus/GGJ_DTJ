using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicManager : MonoBehaviour
{
    [SerializeField] GameObject CameraPlayer;
    [SerializeField] public GameObject CameraCinematic;
    [SerializeField] GameObject Stamina;
    [SerializeField] PlayerController playerController;
    [SerializeField] TimelineAsset timelineAssetStart;
    [SerializeField] TimelineAsset timelineAssetEnd;
    [SerializeField] GameObject Fog;
    PlayableDirector playableDirector;

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
        CameraPlayer.SetActive(false);
        Fog.SetActive(false);
        // Cache or create a PlayableDirector so Play() calls are safe
        playableDirector = GetComponentInChildren<PlayableDirector>() ?? GetComponent<PlayableDirector>();
        if (playableDirector == null)
        {
            playableDirector = gameObject.AddComponent<PlayableDirector>();
        }

        if (timelineAssetStart != null)
        {
            playableDirector.playableAsset = timelineAssetStart;
            playableDirector.Play();
        }
        else
        {
            Debug.LogWarning("CinematicManager: timelineAssetStart is not assigned.");
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        CameraCinematic.SetActive(true);
        Stamina.SetActive(false);
        subtitles.SetActive(false);
        //wait for the end of the timeline
        StartCoroutine(WaitForCinematicEnd());
    }

    IEnumerator WaitForCinematicEnd()
    {
        yield return new WaitForSeconds(6f); // Simulated cinematic duration
        subtitles.SetActive(true);
        Say();
        yield return new WaitForSeconds(19f);
        CameraCinematic.SetActive(false);
        CameraPlayer.SetActive(true);

        playerController.enabled = true;
        CharacterController characterController = playerController.GetComponent<CharacterController>();
        if (characterController != null)
            characterController.enabled = true;

    }

    public void PlayEnd()
    {
        Fog.SetActive(true);

        if (playableDirector == null)
            playableDirector = GetComponentInChildren<PlayableDirector>();

        if (playableDirector == null)
        {
            Debug.LogError("CinematicManager: No PlayableDirector found.");
            return;
        }

        if (timelineAssetEnd == null)
        {
            Debug.LogWarning("CinematicManager: timelineAssetEnd not assigned.");
            return;
        }

        // STOP la timeline actuelle
        playableDirector.Stop();

        // Change la timeline
        playableDirector.playableAsset = timelineAssetEnd;

        // Reset le temps
        playableDirector.time = 0;

        // Applique l'état immédiatement
        playableDirector.Evaluate();

        // Joue la nouvelle timeline
        playableDirector.Play();
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
        Stamina.SetActive(true);
        audioSource.Stop();
        Destroy(audioSource);

        // Re-enable input so the player can move
        InputManager inputManager = InputManager.Instance;
        if (inputManager != null)
        {
            inputManager.EnableAllInputs();
        }

        Timer.Instance.StartTimer();
        MusicManager.instance.PlayMusic();
        playerController.StartPlay();
        gameObject.SetActive(false);
    }

    private IEnumerator PlayAudioWithSubtitles(AudioObject clip)
    {
        audioSource.PlayOneShot(clip.audioClip);
        SetSubtitle(clip.subtitle);
        yield return new WaitForSeconds(clip.audioClip.length);
        ClearSubtitle();
    }

}
