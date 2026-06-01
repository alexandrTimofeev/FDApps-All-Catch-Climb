using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioSource audioSource;
    public void StartPlaying()
    {
        audioSource.volume = PlayerPrefs.GetFloat(nameof(PlayerPrefsEnums.MusicVolume), 0.5f);
        audioSource.clip = musicClip;
        audioSource.loop = true;

        audioSource.Play();
    }
    public void ChangeVolume(float volume) => audioSource.volume = volume;
}
