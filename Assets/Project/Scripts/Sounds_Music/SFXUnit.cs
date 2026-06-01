using UnityEngine;
using DG.Tweening;

public class SFXUnit : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private SoundPlayer soundPlayer;
    public void Initialize(SoundPlayer soundPlayer)
    {
        this.soundPlayer = soundPlayer;
    }
    public void PlaySfx(AudioClip audioClip, float volume)
    {
        audioSource.PlayOneShot(audioClip, volume);

        DOVirtual.DelayedCall(audioClip.length, () => soundPlayer.ReturnSFXobject(this));
    }
}
