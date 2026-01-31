using UnityEngine;

public class CleanMuzzle : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 2f);
    }
}
