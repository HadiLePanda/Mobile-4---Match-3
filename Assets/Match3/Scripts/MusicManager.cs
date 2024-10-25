using System.Collections;
using UnityEngine;

public class MusicManager : SingletonMonoBehaviour<MusicManager>
{
    [Header("References")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;

    [Header("Settings")]
    public float defaultMusicVolume = 0.5f;
    public float maximumMusicVolumeDb = -15f;
    public AudioClip gameMusic;
    public AudioClip menuMusic;
    public AudioClip gameAmbient;

    private Coroutine musicFadeRoutine;
    private Coroutine ambientFadeRoutine;

    protected override bool IsDontDestroyOnLoad() => true;

    // AMBIENT
    public void PlayAmbient(AudioClip clip, float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        if (gameAmbient == null)
            return;

        if (ambientFadeRoutine != null)
            StopCoroutine(ambientFadeRoutine);
        ambientFadeRoutine = StartCoroutine(AnimateSourceCrossfade(ambientSource, clip, targetVolume, fadeDuration));
    }
    public void StopAmbient()
    {
        PlayAmbient(null, 0);
    }

    // MUSIC
    public void PlayMusic(AudioClip clip, float targetVolume = 1f, float fadeDuration = 0.5f, float fadeOutDuration = 0.5f)
    {
        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);
        musicFadeRoutine = StartCoroutine(AnimateSourceCrossfade(musicSource, clip, targetVolume, fadeDuration, fadeOutDuration));
    }
    public void PlayMusicInstant(float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        if (gameMusic == null)
            return;

        musicSource.clip = gameMusic;
        musicSource.Play();
    }
    public void StopMusic()
    {
        PlayMusic(null, 0);
    }
    public void StopMusicInstant() => musicSource.Stop();
    public void PauseMusic() => musicSource.Pause();
    public void ResumeMusic() => musicSource.UnPause();
    public void SetMusicVolume(float volume) => musicSource.volume = volume;

    IEnumerator AnimateSourceCrossfade(AudioSource audioSource, AudioClip nextTrack, float targetVolume = 1f, float fadeDuration = 0.5f, float fadeOutDuration = 0.5f)
    {
        float percent = 0;

        // if we have a track currenly playing, fade it out first
        if (audioSource.isPlaying)
        {
            float startingVolume = audioSource.volume;
            while (percent < 1)
            {
                percent += Time.deltaTime * 1 / fadeOutDuration;
                audioSource.volume = Mathf.Lerp(startingVolume, 0, percent);
                yield return null;
            }
            audioSource.Stop();
        }

        // if we have a next track to play, play it with fade in
        if (nextTrack != null)
        {
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
}
