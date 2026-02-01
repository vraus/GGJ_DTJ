using TMPro;
using UnityEngine;

public class MaskLoad : MonoBehaviour
{
    private static MaskLoad instance;

    public static MaskLoad Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] TextMeshProUGUI maskGoalText;
    int currentMasks = 0;
    int maximumMasks = 3;

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

    public void IncrementeMaskLoad()
    {
        currentMasks++;
        UpdateDisplayText();
    }

    public void DecrementeMaskLoad()
    {
        currentMasks--;
        UpdateDisplayText();
    }

    public void UpdateDisplayText()
    {
        maskGoalText.text = string.Format("{0}/{1}", currentMasks, maximumMasks);
    }
}
