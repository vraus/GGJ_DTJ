using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ToggleOldVideo : MonoBehaviour
{
    [SerializeField] Sprite EmptyImage;
    [SerializeField] Sprite OldVideoImage;
    [SerializeField] Button button;
    private bool isOldVideo = false;
    public UniversalRendererData rendererData;

    public void SetOldEffect(bool enabled)
    {
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature.name == "FullScreenPassRendererFeature")
            {
                feature.SetActive(enabled);
            }
        }
    }

    void Start()
    {
        int oldVideoPref = PlayerPrefs.GetInt("OldVideoEffect", 0);
        if (oldVideoPref == 1)
        {
            isOldVideo = true;
            button.GetComponent<Image>().sprite = OldVideoImage;
        }
        else
        {
            isOldVideo = false;
            button.GetComponent<Image>().sprite = EmptyImage;
        }
    }

    public void ToggleOldVideoEffect()
    {
        isOldVideo = !isOldVideo;
        if (isOldVideo)
        {
            button.GetComponent<Image>().sprite = OldVideoImage;
            PlayerPrefs.SetInt("OldVideoEffect", 1);
            SetOldEffect(true);
        }
        else
        {
            button.GetComponent<Image>().sprite = EmptyImage;
            PlayerPrefs.SetInt("OldVideoEffect", 0);
            SetOldEffect(false);
        }
    }
}
