using UnityEngine;

public class MenuPause : MonoBehaviour
{
    public void BackToMenu()
    {
        Time.timeScale = 1f; // Resume game time
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_Menu");
    }
}
