using Unity.VisualScripting;
using UnityEngine;

public class MortierManager : MonoBehaviour
{
    [SerializeField] GameObject MortierShadow;
    [SerializeField] GameObject MortierExplosion;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject SmokeEffect;
    [SerializeField] GameObject Target;

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
            if (spawnTimer >= spawnInterval && Player.transform.position.z < Target.transform.position.z)
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
        Vector3 spawnBasePos = new Vector3(playerPos.x + randomCircle.x, playerPos.y + 100f, playerPos.z + randomCircle.y);

        // Raycast down to find terrain height
        float spawnHeight = playerPos.y;
        RaycastHit hit;
        if (Physics.Raycast(spawnBasePos, Vector3.down, out hit, 200f))
        {
            spawnHeight = hit.point.y + 0.2f;
        }

        Vector3 spawnPos = new Vector3(spawnBasePos.x, spawnHeight, spawnBasePos.z);

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
        CheckDistanceToPlayer();
        Invoke("DisableMortierExplosion", 1f);
    }

    void CheckDistanceToPlayer()
    {
        float distance = Vector3.Distance(MortierExplosion.transform.position, Player.transform.position);
        PlayerController playerController = Player.GetComponent<PlayerController>();
        switch (distance)
        {
            case float d when d < 2f:
                playerController.CameraShake(5f);
                playerController.PlayerDeath();
                break;
            case float d when d < 5f:
                playerController.CameraShake(2f);
                break;
            default:
                playerController.CameraShake(1f);
                break;
        }
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
