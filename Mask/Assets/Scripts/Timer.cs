using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{

    private static Timer instance;

    public static Timer Instance
    {
        get
        {
            return instance;
        }
    }

    [Header("Components")]
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float timer = 300f;
    bool isActive = false;
    private float timeRemaining;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        timeRemaining = timer;
        UpdateDisplay();
        timerText.gameObject.SetActive(false);
    }

    public void StartTimer()
    {
        timerText.gameObject.SetActive(true);
        isActive = true;
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        while (timeRemaining > 0 && isActive)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            UpdateDisplay();
        }
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        int minutes = (int)(timeRemaining / 60);
        int seconds = (int)(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
