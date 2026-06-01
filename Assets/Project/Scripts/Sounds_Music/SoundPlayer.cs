using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public class SoundPlayer : MonoBehaviour
{
    private readonly int startSize = 15;
    [SerializeField] private Transform sfxUnitPf;
    [SerializeField] private Transform playerTR;
    private SFXManager _SFXManager;

    private Queue<SFXUnit> sfxUnits = new();

    [Inject]
    public void Construct(SFXManager sfxManager)
    {
        _SFXManager = sfxManager;
    }

    void Awake()
    {
        InitializeSFXunits();
    }

    private void InitializeSFXunits()
    {
        if (sfxUnitPf == null)
        {
            Debug.LogError($"{nameof(SoundPlayer)}.{nameof(InitializeSFXunits)}: {nameof(sfxUnitPf)} is null on {name}.");
            return;
        }

        for (int i = 0; i < startSize; i++)
        {
            var sfxObject = Instantiate(sfxUnitPf, transform);
            var sfxUnit = sfxObject.GetComponent<SFXUnit>();
            if (sfxUnit == null)
            {
                Debug.LogError($"{nameof(SoundPlayer)}.{nameof(InitializeSFXunits)}: {nameof(SFXUnit)} is null on {sfxObject.name}.");
                Destroy(sfxObject.gameObject);
                continue;
            }

            sfxUnit.Initialize(this);

            sfxUnit.gameObject.SetActive(false);
            sfxUnits.Enqueue(sfxUnit);
        }
    }

    public void PlaySFX(SFXType sfxType, Vector3? position = null, float volume = 1f)
    {
        _ = PlaySFXAsync(sfxType, position, volume);
    }

    public void PlayCoinSFX(Vector3? position = null, float volume = 1f)
    {
        _ = PlayCoinSFXAsync(position, volume);
    }

    private async Task PlaySFXAsync(SFXType sfxType, Vector3? position = null, float volume = 1f)
    {
        if (_SFXManager == null)
        {
            Debug.LogError($"{nameof(SoundPlayer)}.{nameof(PlaySFXAsync)}: {nameof(_SFXManager)} is null on {name}.");
            return;
        }

        AudioClip clip = await _SFXManager.GetClipAsync(sfxType);
        PlayClipInternal(clip, position, volume);
    }

    private async Task PlayCoinSFXAsync(Vector3? position = null, float volume = 1f)
    {
        if (_SFXManager == null)
        {
            Debug.LogError($"{nameof(SoundPlayer)}.{nameof(PlayCoinSFXAsync)}: {nameof(_SFXManager)} is null on {name}.");
            return;
        }

        AudioClip clip = await _SFXManager.GetRandomCoinClipAsync();
        PlayClipInternal(clip, position, volume);
    }

    private void PlayClipInternal(AudioClip clip, Vector3? position, float volume)
    {
        if (clip == null)
        {
            Debug.LogError("Audio clip was not found");
            return;
        }

        SFXUnit unit;
        if (sfxUnits.Count == 0)
        {
            if (sfxUnitPf == null)
            {
                Debug.LogError($"{nameof(SoundPlayer)}.{nameof(PlayClipInternal)}: {nameof(sfxUnitPf)} is null on {name}.");
                return;
            }

            var sfxObject = Instantiate(sfxUnitPf, transform);
            unit = sfxObject.GetComponent<SFXUnit>();
            if (unit == null)
            {
                Debug.LogError($"{nameof(SoundPlayer)}.{nameof(PlayClipInternal)}: {nameof(SFXUnit)} is null on {sfxObject.name}.");
                Destroy(sfxObject.gameObject);
                return;
            }

            unit.Initialize(this);
        }
        else
        {
            unit = sfxUnits.Dequeue();
        }

        unit.gameObject.SetActive(true);
        unit.transform.position = position ?? (playerTR != null ? playerTR.position : transform.position);
        unit.PlaySfx(clip, volume);
    }

    public void ReturnSFXobject(SFXUnit unit)
    {
        unit.gameObject.SetActive(false);
        sfxUnits.Enqueue(unit);
    }
}
