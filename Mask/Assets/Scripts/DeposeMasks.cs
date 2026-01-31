using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DeposeMasks : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DropText;
    [SerializeField] List<GameObject> dropZones;

    void Start()
    {
        DropText.gameObject.SetActive(false);
        foreach (var zone in dropZones)
        {
            zone.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                DropText.transform.LookAt(other.transform);
                DropText.gameObject.SetActive(true);
                DropText.transform.Rotate(0, 180, 0);
            }
            else
            {
                DropText.gameObject.SetActive(false);
            }

        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var pc = other.GetComponent<PlayerController>();
            if (pc != null && pc.MasksCollected > 0)
            {
                DropText.transform.LookAt(other.transform);
                DropText.gameObject.SetActive(true);
                DropText.transform.Rotate(0, 180, 0);

                if (pc.MasksCollected > 0 && InputManager.Instance.IsDroppedPressed()) //player is facing the drop zone
                {
                    GameObject mask = dropZones.FirstOrDefault(z => z.activeSelf == false);
                    mask.SetActive(true);
                    pc.MasksCollected--;
                    //soldats qui disent merci ?
                    //Bruit de dépot de masques
                }
                else
                {
                    DropText.gameObject.SetActive(false);
                }
            }
        }
    }
}
