using UnityEngine;

public class MenuPause : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("References")]
    [SerializeField] private PlayerController playerController;

    void Start()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
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

    public void ResumeGame()
    {
        if (playerController != null)
        {
            playerController.ResumeGame();
        }
    }
}
