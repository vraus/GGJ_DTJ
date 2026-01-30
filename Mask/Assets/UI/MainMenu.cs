using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Scene_Gameplay");
    }

    public void Settings()
    {
        SceneManager.LoadScene("Scene_Settings");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
