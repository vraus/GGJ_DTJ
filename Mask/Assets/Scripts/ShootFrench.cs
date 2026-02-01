using System;
using System.Collections;
using UnityEngine;

public class ShootFrench : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] GameObject Muzzle;
    [SerializeField] float destroyAfter = 2f;
    [SerializeField] AudioClip shootSound;
    [SerializeField] GameObject Mask;
    private GameObject instance;

    void Start()
    {
        if (target != null && Muzzle != null)
        {
            instance = Instantiate(target, Muzzle.transform.position, Muzzle.transform.rotation);
            instance.SetActive(false);

        }
        if (Mask != null)
        {
            Mask.SetActive(false);
        }
    }

    public void Shoot()
    {
        if (target == null || Muzzle == null || instance == null)
            return;

        instance.SetActive(true);
        SoundFXManager.instance.PlayAudioClip(shootSound, transform, 1f);
        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        instance.SetActive(false);
    }

    public void WearMask()
    {
        Mask.SetActive(true);
    }
}
