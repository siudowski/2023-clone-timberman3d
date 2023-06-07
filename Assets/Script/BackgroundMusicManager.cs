using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    float maxAudioVolume;
    float volumeTransitionDuration;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameManager.instance.OnStateChanged += GameManager_OnStateChanged;

        maxAudioVolume = GameManager.instance.MaxAudioVolume;
        volumeTransitionDuration = GameManager.instance.VolumeTransitionDuration;
    }

    private void OnDestroy()
    {
        GameManager.instance.OnStateChanged -= GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Default:
                audioSource.Play();
                StartCoroutine(StartEase(volumeTransitionDuration, maxAudioVolume, false));
                break;
            case GameState.End:
                StartCoroutine(StartEase(volumeTransitionDuration, 0, true));
                break;
            default:
                break;
        }
    }

    IEnumerator StartEase(float duration, float targetVolume, bool isStop)
    {
        if (!isStop)
            audioSource.Play();
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        if (isStop)
            audioSource.Stop();
        yield break;
    }
}
