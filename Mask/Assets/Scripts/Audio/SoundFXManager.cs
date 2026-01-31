using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance { get; private set; }

    [SerializeField] private AudioSource footstepSourcePrefab;
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

    void CreateFootstepSource()
    {
        FootstepSource = Instantiate(footstepSourcePrefab, transform);
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
