using System.Collections;
using UnityEngine;

public class MusicManager : SingletonMonoBehaviour<MusicManager>
{
    [Header("References")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;

    [Header("Settings")]
    public AudioClip gameMusic;
    public AudioClip menuMusic;
    public AudioClip gameAmbient;
    public float defaultMusicVolume = 0.5f;

    private Coroutine musicFadeRoutine;
    private Coroutine ambientFadeRoutine;

    protected override bool IsDontDestroyOnLoad() => true;

    // AMBIENT
    public void PlayAmbient(float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        if (gameAmbient == null)
            return;

        if (ambientFadeRoutine != null)
            StopCoroutine(ambientFadeRoutine);
        ambientFadeRoutine = StartCoroutine(AnimateSourceCrossfade(ambientSource, gameAmbient, targetVolume, fadeDuration));
    }

    // MUSIC
    public void PlayMusic(AudioClip clip, float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);
        musicFadeRoutine = StartCoroutine(AnimateSourceCrossfade(musicSource, clip, targetVolume, fadeDuration));
    }
    public void PlayMusicInstant(float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        if (gameMusic == null)
            return;

        musicSource.clip = gameMusic;
        musicSource.Play();
    }
    public void StopMusic() => musicSource.Stop();
    public void PauseMusic() => musicSource.Pause();
    public void ResumeMusic() => musicSource.UnPause();
    public void SetMusicVolume(float volume) => musicSource.volume = volume;

    IEnumerator AnimateSourceCrossfade(AudioSource audioSource, AudioClip nextTrack, float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        float percent = 0;
        float startingVolume = audioSource.volume;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            audioSource.volume = Mathf.Lerp(startingVolume, 0, percent);
            yield return null;
        }

        audioSource.clip = nextTrack;
        audioSource.Play();

        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            audioSource.volume = Mathf.Lerp(0, targetVolume, percent);
            yield return null;
        }
    }
}
