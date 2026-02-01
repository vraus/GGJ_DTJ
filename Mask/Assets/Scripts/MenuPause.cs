using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuPause : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject stamina;
    [SerializeField] private GameObject LoseMenu;
    [SerializeField] private GameObject WinMenu;

    [Header("Audio")]
    [SerializeField] private Slider MasterVolumeSlider;
    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;
    [SerializeField] private SoundMixerManager soundMixerManager;

    [Header("Audio Clip")]
    [SerializeField] private AudioClip loseAudioClip;
    [SerializeField] private AudioClip winAudioClip;

    List<GameObject> FrenchSoldiers = new List<GameObject>();
    [SerializeField] CinematicManager cinematicManager;

    void Start()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            LoseMenu.SetActive(false);
        }

        //get all gameobjects with tag FrenchSoldier
        FrenchSoldiers.AddRange(GameObject.FindGameObjectsWithTag("French"));

        if (soundMixerManager != null)
        {
            // Initialize volume sliders with current mixer values
            float masterVolume, musicVolume, sfxVolume;
            soundMixerManager.audioMixer.GetFloat("masterVolume", out masterVolume);
            soundMixerManager.audioMixer.GetFloat("musicVolume", out musicVolume);
            soundMixerManager.audioMixer.GetFloat("sfxVolume", out sfxVolume);

            MasterVolumeSlider.value = masterVolume;
            MusicVolumeSlider.value = musicVolume;
            SFXVolumeSlider.value = sfxVolume;
        }
    }

    public void TogglePauseMenu(bool isPaused)
    {
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenuUI.SetActive(isPaused);
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_Menu");
    }

    public void TimerStopped()
    {
        if (playerController.GetCurretnTotalCollectedMasksWinCondition())
            DisplayWinMenu();
        else
            playerController.PlayerDeath();
    }

    public void DisplayLoseMenu()
    {
        LoseMenu.SetActive(true);
        MusicManager.instance.StopMusic();

        foreach (GameObject soldier in FrenchSoldiers)
        {
            soldier.SetActive(false);
        }
        playerController.GetComponentInChildren<Camera>().gameObject.SetActive(false);
        cinematicManager.gameObject.SetActive(true);
        cinematicManager.CameraCinematic.SetActive(true);
        cinematicManager.PlayEnd();

        stamina.SetActive(false);
        PlayEndgameSound(loseAudioClip);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void DisplayWinMenu()
    {
        foreach (GameObject soldier in FrenchSoldiers)
        {
            soldier.GetComponent<ShootFrench>().WearMask();
        }

        //get cinematic manager and play end cinematic
        playerController.GetComponentInChildren<Camera>().gameObject.SetActive(false);
        cinematicManager.gameObject.SetActive(true);
        cinematicManager.CameraCinematic.SetActive(true);
        cinematicManager.PlayEnd();

        WinMenu.SetActive(true);
        MusicManager.instance.StopMusic();
        stamina.SetActive(false);
        PlayEndgameSound(winAudioClip);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void PlayEndgameSound(AudioClip audioClip)
    {
        SoundFXManager.instance.PlayAudioClip(audioClip, playerController.transform, 1f);
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_Gameplay");
    }

    public void ResumeGame()
    {
        if (playerController != null)
        {
            playerController.ResumeGame();
        }
    }
}
