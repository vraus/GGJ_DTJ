using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class CinematicManager : MonoBehaviour
{
    [SerializeField] GameObject CameraPlayer;
    [SerializeField] GameObject CameraCinematic;
    [SerializeField] GameObject Stamina;
    [SerializeField] PlayerController playerController;
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
        CameraCinematic.SetActive(true);
        CameraPlayer.SetActive(false);
        Stamina.SetActive(false);
        subtitles.SetActive(false);
        //wait for the end of the timeline
        StartCoroutine(WaitForCinematicEnd());
    }

    IEnumerator WaitForCinematicEnd()
    {
        yield return new WaitForSeconds(6f); // Simulated cinematic duration
        subtitles.SetActive(true);
        CameraCinematic.SetActive(false);
        CameraPlayer.SetActive(true);

        playerController.enabled = true;
        CharacterController characterController = playerController.GetComponent<CharacterController>();
        if (characterController != null)
            characterController.enabled = true;

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
        Stamina.SetActive(true);
        audioSource.Stop();
        Destroy(audioSource);

        // Re-enable input so the player can move
        InputManager inputManager = InputManager.Instance;
        if (inputManager != null)
        {
            inputManager.EnableAllInputs();
        }

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
