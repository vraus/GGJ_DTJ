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
    private bool isEndCinematicPlaying = false;
    private Coroutine endCinematicCoroutine;

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
            Debug.Log("CinematicManager: Playing start timeline");
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
        Debug.Log("CinematicManager: PlayEnd called");

        Fog.SetActive(true);

        // Ensure the cinematic camera is active and player camera / HUD are disabled
        if (CameraCinematic != null) CameraCinematic.SetActive(true);
        if (CameraPlayer != null) CameraPlayer.SetActive(false);
        if (Stamina != null) Stamina.SetActive(false);

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

        // Stop current timeline, assign the end timeline and play it
        Debug.Log("CinematicManager: Starting end cinematic, timeline duration: " + timelineAssetEnd.duration);
        playableDirector.Stop();
        playableDirector.playableAsset = timelineAssetEnd;
        playableDirector.time = 0;
        playableDirector.Play(timelineAssetEnd);

        isEndCinematicPlaying = true;
        if (endCinematicCoroutine != null)
            StopCoroutine(endCinematicCoroutine);
        endCinematicCoroutine = StartCoroutine(WaitForEndCinematicFinish());
    }

    private IEnumerator WaitForEndCinematicFinish()
    {
        yield return new WaitForSeconds((float)timelineAssetEnd.duration + 0.5f);

        if (!isEndCinematicPlaying) yield break;

        Debug.Log("CinematicManager: End cinematic finished");
        isEndCinematicPlaying = false;
        CameraCinematic.SetActive(false);
        CameraPlayer.SetActive(true);
        gameObject.SetActive(false);
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
