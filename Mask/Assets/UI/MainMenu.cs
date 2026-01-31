using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject settingsUI;
    [SerializeField] GameObject creditsUI;
    [SerializeField] GameObject warningUI;
    bool start;

    void Start()
    {
        settingsUI.SetActive(false);
        mainMenuUI.SetActive(false);
        creditsUI.SetActive(false);
        warningUI.SetActive(false);
    }

    void Update()
    {
        if (!start)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            {
                start = true;
                Menu();
            }
            return;
        }
    }

    public void Play()
    {
        SceneManager.LoadScene("Scene_Gameplay");
    }

    public void Settings()
    {
        settingsUI.SetActive(true);
        mainMenuUI.SetActive(false);
        creditsUI.SetActive(false);
        warningUI.SetActive(false);
    }

    public void Menu()
    {
        mainMenuUI.SetActive(true);
        settingsUI.SetActive(false);
        creditsUI.SetActive(false);
        warningUI.SetActive(false);
    }

    public void Credits()
    {
        creditsUI.SetActive(true);
        mainMenuUI.SetActive(false);
        settingsUI.SetActive(false);
        warningUI.SetActive(false);
    }

    public void Warning()
    {
        warningUI.SetActive(true);
        mainMenuUI.SetActive(false);
        settingsUI.SetActive(false);
        creditsUI.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
