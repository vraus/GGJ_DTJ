using UnityEngine;

public class DeleteSmoke : MonoBehaviour
{
    internal void Begin()
    {
        Destroy(gameObject, 5f);
    }
}
