using UnityEngine;

public class CollectMask : MonoBehaviour
{
    public GameObject infoText;
    public float maxLookDistance = 3f;

    private Transform player;

    void Start()
    {
        infoText.SetActive(false);
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= maxLookDistance)
        {
            infoText.SetActive(true);

            infoText.transform.LookAt(player);
            // If player presses collect while info is shown, collect the mask
            var inputMgr = InputManager.Instance;
            if (inputMgr != null && inputMgr.IsCollectPressed())
            {
                var pc = player.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.AddMask();
                }
                Destroy(gameObject);
            }
        }
        else
        {
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
