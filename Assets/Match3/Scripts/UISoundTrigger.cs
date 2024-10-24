using UnityEngine;

public class UISoundTrigger : MonoBehaviour
{
    [Header("References")]
    public AudioClip clip;
    public float pitchVariation = 0.05f;

    public void PlaySound()
    {
        AudioManager.Instance.PlaySoundUIOneShot(clip, pitchVariation: pitchVariation);
    }
}
