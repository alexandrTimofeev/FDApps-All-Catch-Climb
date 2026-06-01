using UnityEngine;
using VContainer;

public class OceanAmbient : MonoBehaviour
{
    [SerializeField] private SpriteRenderer borderBox;
    [SerializeField] private AudioSource audioSource;
    [Header("Stats")]
    [SerializeField] private float followSpeed;
    [SerializeField] private float maxDistance = 30f;
    [SerializeField] private float minimumVolume = 0.2f;
    private PlayerController playerController;
    private Transform audioSourceTR;
    private float maxVolume;
    Vector3[] corners = new Vector3[4];

    [Inject]
    public void Construct(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    void Awake()
    {
        if (audioSource == null)
        {
            Debug.LogError($"{nameof(OceanAmbient)}.{nameof(Awake)}: {nameof(audioSource)} is null on {name}.");
            enabled = false;
            return;
        }

        if (borderBox == null)
        {
            Debug.LogError($"{nameof(OceanAmbient)}.{nameof(Awake)}: {nameof(borderBox)} is null on {name}.");
            enabled = false;
            return;
        }

        audioSourceTR = audioSource.transform;

        maxVolume = audioSource.volume;

        CalculateBoundries();
    }
    void Update()
    {
        if (playerController == null)
        {
            Debug.LogError($"{nameof(OceanAmbient)}.{nameof(Update)}: {nameof(playerController)} is null on {name}.");
            return;
        }

        MoveAudioSource();
    }
    private void MoveAudioSource()
    {
        Vector3 targetPosition = playerController.GetPlayerPosition();

        targetPosition.x = Mathf.Clamp(targetPosition.x, corners[0].x, corners[1].x);
        targetPosition.z = Mathf.Clamp(targetPosition.z, corners[1].z, corners[2].z);

        audioSourceTR.position = Vector3.Lerp(audioSourceTR.position, targetPosition, followSpeed * Time.deltaTime);

        float distance = Vector3.Distance(audioSourceTR.position, playerController.GetPlayerPosition());
        audioSource.volume = Mathf.Clamp(1f - distance / maxDistance, minimumVolume, maxVolume);
    }
    private void CalculateBoundries()
    {
        Bounds bounds = borderBox.bounds;

        float fixedY = borderBox.transform.position.y;

        corners[0] = new Vector3(bounds.min.x, fixedY, bounds.min.z);
        corners[1] = new Vector3(bounds.max.x, fixedY, bounds.min.z);
        corners[2] = new Vector3(bounds.max.x, fixedY, bounds.max.z);
        corners[3] = new Vector3(bounds.min.x, fixedY, bounds.max.z);
    }
}
