using UnityEngine;

public class ShootFrench : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] GameObject Muzzle;
    [SerializeField] float destroyAfter = 2f;
    [SerializeField] AudioClip shootSound;

    public void Shoot()
    {
        GameObject instance = Instantiate(target, Muzzle.transform.position, Muzzle.transform.rotation);
        SoundFXManager.instance.PlayAudioClip(shootSound, transform, 1f);
        if (destroyAfter > 0f)
            Destroy(instance, destroyAfter);
    }
}
