using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

public class PlayerStepsSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float stepDelay = 0.5f;
    private TimerManager timerManager;
    private List<AudioClip> sfxList;
    private SmartTimer stepTimer;

    [Inject]
    public void Construct(TimerManager timerManager)
    {
        this.timerManager = timerManager;
    }

    private async void Start()
    {
        await LoadSFXAsync();
        InitializeTimer();
    }

    private async UniTask LoadSFXAsync()
    {
        AsyncOperationHandle<IList<AudioClip>> handle = Addressables.LoadAssetsAsync<AudioClip>("SandStepSFX", null);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            sfxList = new List<AudioClip>(handle.Result);
        }
        else
        {
            Debug.LogError("Failed to load SandStepSFX list from Addressables.");
            return;
        }
    }

    private void InitializeTimer()
    {
        if (timerManager == null)
        {
            Debug.LogError($"{nameof(PlayerStepsSound)}.{nameof(InitializeTimer)}: {nameof(timerManager)} is null on {name}.");
            return;
        }

        if (stepTimer == null)
        {
            stepTimer = timerManager.CreateTimer(stepDelay, PlayRandomStepSound);
            stepTimer.IsLooped = true;
            stepTimer.TickCondition = () => gameObject.activeInHierarchy && audioSource != null;
            stepTimer.Pause();
        }
    }

    public void StartPlaying()
    {
        if (sfxList == null || sfxList.Count == 0)
        {
            Debug.LogWarning("No step sounds available to play.");
            return;
        }

        if (stepTimer == null)
        {
            InitializeTimer();
        }

        stepTimer.Reset();
        stepTimer.Resume();
    }

    private void PlayRandomStepSound()
    {
        if (sfxList.Count > 0 && audioSource != null)
        {
            AudioClip clip = sfxList[Random.Range(0, sfxList.Count)];
            audioSource.PlayOneShot(clip);
        }

        stepTimer.Reset();
    }

    public void StopPlaying()
    {
        if (stepTimer != null)
        {
            stepTimer.Pause();
        }
    }

    private void OnDestroy()
    {
        if (stepTimer != null)
        {
            stepTimer.Stop();
            stepTimer = null;
        }
    }
}
