using Unity.VisualScripting;
using UnityEngine;

public class PoisonGaz : MonoBehaviour
{
    Collider collider;

    void Start()
    {
        collider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.PlayerInGazArea(true);
                Debug.Log("Player entered gaz area.");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.PlayerInGazArea(false);
                Debug.Log("Player left gaz area.");
            }
        }
    }
}
