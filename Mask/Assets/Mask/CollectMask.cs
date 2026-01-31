using UnityEngine;

public class CollectMask : MonoBehaviour
{
    public GameObject infoText;
    public GameObject CantCarryText;
    public GameObject Canvas;
    public float maxLookDistance = 3f;

    private Transform player;

    void Start()
    {
        infoText.SetActive(false);
        CantCarryText.SetActive(false);
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);
        Canvas.transform.LookAt(player);
        Canvas.transform.Rotate(0, 180, 0);

        if (distance <= maxLookDistance)
        {

            if (player.GetComponent<PlayerController>().MasksCollected >= player.GetComponent<PlayerController>().MaxMasksCarriable)
            {
                CantCarryText.SetActive(true);
                infoText.SetActive(false);
                return;
            }
            else
            {
                infoText.SetActive(true);
                CantCarryText.SetActive(false);
            }



            // If player presses collect while info is shown, collect the mask
            var inputMgr = InputManager.Instance;
            if (inputMgr != null && inputMgr.IsCollectPressed())
            {
                var pc = player.GetComponent<PlayerController>();
                if (pc != null && pc.MasksCollected < pc.MaxMasksCarriable)
                {
                    pc.AddMask();
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            CantCarryText.SetActive(false);
            infoText.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            infoText.SetActive(false);
        }
    }
}
