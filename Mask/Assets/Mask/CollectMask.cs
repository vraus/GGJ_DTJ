using UnityEngine;
using TMPro;

public class CollectMask : MonoBehaviour
{
    public GameObject infoText;
    public float maxLookDistance = 3f;
    bool playerNearby = false;

    void Start()
    {
        infoText.SetActive(false);
    }

    void Update()
    {
        if (!playerNearby) return;

        if (!playerNearby && infoText.activeSelf)
            infoText.SetActive(false);

        infoText.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            infoText.SetActive(false);
        }
    }
}
