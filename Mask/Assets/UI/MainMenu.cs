using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject settingsUI;
    [SerializeField] GameObject creditsUI;

    void Start()
    {
        Menu();
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
    }

    public void Menu()
    {
        mainMenuUI.SetActive(true);
        settingsUI.SetActive(false);
        creditsUI.SetActive(false);
    }

    public void Credits()
    {
        creditsUI.SetActive(true);
        mainMenuUI.SetActive(false);
        settingsUI.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
