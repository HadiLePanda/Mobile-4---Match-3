using System.Collections;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [Header("References")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    [Header("Settings")]
    public float defaultMasterVolume = 1f;

    protected override bool IsDontDestroyOnLoad() => true;

    // UI
    public void PlaySoundUIOneShot(AudioClip clip, float volume = 1.0f, float pitchVariation = 0f)
    {
        if (clip == null)
            return;

        PlayOneShot(uiSource, clip, volume, pitchVariation);
    }
    // SFX
    public void PlaySound2DOneShot(AudioClip clip, float volume = 1.0f, float pitchVariation = 0f)
    {
        if (clip == null)
            return;

        PlayOneShot(sfxSource, clip, volume, pitchVariation);
    }
    public void PlaySound2DOneShotWithDelay(AudioClip clip, float volume = 1.0f, float pitchVariation = 0f, float delay = 0f)
    {
        if (clip == null)
            return;

        StartCoroutine(TriggerSound2DOneShotAfterDelay(clip, volume, pitchVariation, delay));
    }
    private IEnumerator TriggerSound2DOneShotAfterDelay(AudioClip clip, float volume = 1.0f, float pitchVariation = 0f, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        PlaySound2DOneShot(clip, volume, pitchVariation);
    }

    private void PlayOneShot(AudioSource audioSource, AudioClip clip, float volume = 1.0f, float pitchVariation = 0f)
    {
        // randomize sound values and play it
        audioSource.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);

        // play sound
        audioSource.PlayOneShot(clip, volume);

        // reset default values
        audioSource.pitch = 1f;
    }
}
