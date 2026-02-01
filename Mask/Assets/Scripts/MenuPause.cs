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
    [SerializeField] private Slider MasterVolumeSlider;
    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;
    [SerializeField] private SoundMixerManager soundMixerManager;

    void Start()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            LoseMenu.SetActive(false);
        }

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

    public void DisplayLoseMenu()
    {
        Time.timeScale = 0f;
        LoseMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
