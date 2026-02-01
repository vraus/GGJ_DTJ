using UnityEngine;

public class MenuPause : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject stamina;
    [SerializeField] private GameObject LoseMenu;

    void Start()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            stamina.SetActive(true);
            LoseMenu.SetActive(false);
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
