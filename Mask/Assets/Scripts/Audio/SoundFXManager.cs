using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance { get; private set; }

    [SerializeField] private AudioSource audioSourcePrefab;
    public AudioSource FootstepSource { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            CreateFootstepSource();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayAudioClip(AudioClip audioClip, Transform position, float volume)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab, position.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioClip.length);
    }

    void CreateFootstepSource()
    {
        FootstepSource = Instantiate(audioSourcePrefab, transform);
        FootstepSource.loop = false;
        FootstepSource.playOnAwake = false;
        FootstepSource.spatialBlend = 1f;
    }

    public void PlayFootstep(AudioClip clip, Vector3 position, float volume)
    {
        FootstepSource.transform.position = position;
        FootstepSource.volume = volume;
        FootstepSource.PlayOneShot(clip);
    }
}
