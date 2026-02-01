using UnityEngine;

public class Artillery : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] float playInterval = 20f;

    private AudioSource audioSource;
    private float timeSinceLastPlay = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("Artillery: Aucune AudioSource trouvée sur cet objet!");
        }

        if (particleSystem == null)
        {
            Debug.LogError("Artillery: Aucun ParticleSystem assigné!");
        }

        // Joue immédiatement au démarrage
        PlayEffectAndSound();
        timeSinceLastPlay = 0f;
    }

    private void Update()
    {
        timeSinceLastPlay += Time.deltaTime;

        // Après playInterval secondes, rejou
        if (timeSinceLastPlay >= playInterval)
        {
            PlayEffectAndSound();
            //particleSystem.Stop();
            timeSinceLastPlay = 0f;
        }
    }

    public void PlayEffectAndSound()
    {
        // Restart particle system so it always plays from the beginning
        if (particleSystem != null)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleSystem.Clear();
            particleSystem.Play();
        }

        // Play audio and schedule particle stop when audio ends
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.Play();

            float clipLength = 0f;
            if (audioSource.clip != null)
                clipLength = audioSource.clip.length;

            if (clipLength > 0f && particleSystem != null)
            {
                StopAllCoroutines();
                StartCoroutine(StopParticlesWhenAudioEnds(clipLength));
            }
        }
    }

    private System.Collections.IEnumerator StopParticlesWhenAudioEnds(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (particleSystem != null)
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    public void StopEffectAndSound()
    {
        if (particleSystem != null)
        {
            particleSystem.Stop();
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}

