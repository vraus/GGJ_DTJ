using UnityEngine;

public class SoundFXManager : MonoBehaviour
{

    public static SoundFXManager instance { get; private set; }

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume = 1.0f)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioClip.length;
        Destroy(audioSource.gameObject, clipLength);

    }
}
