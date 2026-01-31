using Unity.VisualScripting;
using UnityEngine;

public class MortierManager : MonoBehaviour
{
    [SerializeField] GameObject MortierPlace;
    [SerializeField] GameObject MortierShadow;
    [SerializeField] GameObject MortierExplosion;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject SmokeEffect;

    [Header("Audio Assets")]
    [SerializeField] AudioClip MortierTrail;
    [SerializeField] AudioClip MortierExplosionSound;
    [SerializeField] AudioClip[] ExplostionImpacts;

    private float spawnTimer = 0f;
    private float spawnInterval;

    void Start()
    {
        MortierShadow.SetActive(false);
        MortierExplosion.SetActive(false);
        spawnInterval = 0f;
    }

    void Update()
    {
        if (Player != null)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnMortierShadow();
                spawnTimer = 0f;
                spawnInterval = Random.Range(5f, 10f);
            }
        }
    }

    void SpawnMortierShadow()
    {
        Vector3 playerPos = Player.transform.position;
        float radius = 5f;
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        Vector3 spawnPos = new Vector3(playerPos.x + randomCircle.x, 3f, playerPos.z + randomCircle.y);

        MortierShadow.transform.position = spawnPos;
        MortierShadow.transform.localScale = Vector3.one * 0.5f;
        MortierShadow.SetActive(true);
        StartCoroutine(LerpShadowScale());
    }

    System.Collections.IEnumerator LerpShadowScale()
    {
        float elapsedTime = 0f;
        float duration = 1f;
        Vector3 startScale = Vector3.one * 0.5f;
        Vector3 endScale = Vector3.one * 2f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            MortierShadow.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        MortierShadow.transform.localScale = endScale;
        SoundFXManager.instance.PlayAudioClip(MortierTrail, MortierShadow.transform, 1f);
        Invoke("SpawnMortierExplosion", 1f);
    }

    void SpawnMortierExplosion()
    {
        Vector3 shadowPos = MortierShadow.transform.position;
        MortierExplosion.transform.position = shadowPos;
        MortierExplosion.SetActive(true);
        MortierShadow.SetActive(false);
        SpawnSmokeEffect();
        SoundFXManager.instance.PlayAudioClip(MortierExplosionSound, MortierShadow.transform, 1f);
        AudioClip impactClip = ExplostionImpacts[Random.Range(0, ExplostionImpacts.Length)];
        SoundFXManager.instance.PlayAudioClip(impactClip, MortierShadow.transform, .1f);
        Invoke("DisableMortierExplosion", 1f);
    }

    void SpawnSmokeEffect()
    {
        Vector3 shadowPos = MortierShadow.transform.position;
        GameObject smoke = Instantiate(SmokeEffect, shadowPos, Quaternion.identity);
        smoke.GetComponent<DeleteSmoke>().Begin();
    }

    void DisableMortierExplosion()
    {
        MortierExplosion.SetActive(false);
    }
}
