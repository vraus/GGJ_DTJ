using TMPro;
using UnityEngine;

public class MaskGoal : MonoBehaviour
{
    private static MaskGoal instance;

    public static MaskGoal Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] TextMeshProUGUI maskGoalText;
    int currentMasks = 0;
    int maximumMasks = 10;

    void Start()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
        maskGoalText.gameObject.SetActive(false);
    }

    public void StartCounting()
    {
        maskGoalText.gameObject.SetActive(true);
    }

    public void SetMaximumMasks(int max)
    {
        maximumMasks = max;
        currentMasks = 0;
        UpdateDisplayText();
    }

    public void IncrementeMaskGoal()
    {
        currentMasks++;
        UpdateDisplayText();
    }

    public void UpdateDisplayText()
    {
        maskGoalText.text = string.Format("{0}/{1}", currentMasks, maximumMasks);
    }

}
