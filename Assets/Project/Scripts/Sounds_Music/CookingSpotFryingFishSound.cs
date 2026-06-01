using UnityEngine;
using UnityEngine.Audio;

public class CookingSpotFryingFishSound : MonoBehaviour
{
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot mutedSnapshot;
    public float transitionTime = 0.3f;
    public AudioSource audioSource;

    void Awake()
    {
        MuteAudio();
    }
    public void MuteAudio()
    {
        mutedSnapshot.TransitionTo(transitionTime);
    }

    public void EnableAudio()
    {
        normalSnapshot.TransitionTo(transitionTime);
    }
}
