using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DeposeMasks : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DropText;
    [SerializeField] List<GameObject> dropZones;
    [SerializeField] float depositCooldown = 0.3f;
    PlayerController pc;

    private float lastDepositTime = -1f;

    void Start()
    {
        DropText.gameObject.SetActive(false);
        foreach (var zone in dropZones)
        {
            zone.SetActive(false);
        }
    }

    void Update()
    {
        DeposesMasks(pc);
    }

    void OnTriggerEnter(Collider other)
    {
        pc = other.GetComponent<PlayerController>();
    }

    void OnTriggerStay(Collider other)
    {
        pc = other.GetComponent<PlayerController>();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DropText.gameObject.SetActive(false);
        }
        pc = null;
    }

    void DeposesMasks(PlayerController other)
    {
        if (other == null)
            return;

        if (other.CompareTag("Player"))
        {
            pc = other;
            if (pc != null && pc.MasksCollected > 0)
            {
                DropText.transform.LookAt(other.transform);
                DropText.gameObject.SetActive(true);
                DropText.transform.Rotate(0, 180, 0);

                // Check if enough time has passed since last deposit
                if (pc.MasksCollected > 0 && InputManager.Instance.IsDroppedPressed() && Time.time >= lastDepositTime + depositCooldown)
                {
                    GameObject mask = dropZones.FirstOrDefault(z => z.activeSelf == false);
                    if (mask != null)
                    {
                        mask.SetActive(true);
                        pc.MasksCollected--;
                        pc.UpdateMaskMissionProgress();
                        lastDepositTime = Time.time;
                    }
                }
            }
        }
    }
}

