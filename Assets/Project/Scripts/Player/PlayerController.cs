using VContainer;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private static readonly int IsFishing = Animator.StringToHash("IsFishing");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    [SerializeField] private Animator animator;

    private PlayerMovement movement;
    private PlayerInventory inventory;
    private PlayerCharacteristics playerCharacteristics;
    private PlayerStepsSound playerStepsSound;
    private PlayerFootstepVFX playerFootstepVFX;
    private FishingRod fishingRod;

    [Inject]
    public void Construct(
        PlayerMovement movement,
        PlayerInventory inventory,
        PlayerCharacteristics playerCharacteristics,
        PlayerStepsSound playerStepsSound,
        PlayerFootstepVFX playerFootstepVFX,
        FishingRod fishingRod)
    {
        this.movement = movement;
        this.inventory = inventory;
        this.playerCharacteristics = playerCharacteristics;
        this.playerStepsSound = playerStepsSound;
        this.playerFootstepVFX = playerFootstepVFX;
        this.fishingRod = fishingRod;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError($"{nameof(PlayerController)}: Animator is missing on {name}");
            enabled = false;
        }
    }

    private void Start()
    {
        if (!ValidateDependencies(nameof(Start)))
            return;

        fishingRod.Hide();
    }

    private void Update()
    {
        if (!ValidateDependencies(nameof(Update)))
            return;

        movement.HandleInput();
    }

    public void MovePlayer()
    {
        movement.MovePlayer();
    }

    public void EnterMovingAnim()
    {
        animator.SetBool(IsFishing, false);
        animator.SetBool(IsMoving, true);
    }

    public void EnterIdleAnim()
    {
        animator.SetBool(IsFishing, false);
        animator.SetBool(IsMoving, false);
    }

    public void EnterFishingAnim()
    {
        animator.SetBool(IsFishing, true);
    }

    public PlayerMovement GetPlayerMovement() => movement;
    public PlayerInventory GetPlayerInventory() => inventory;
    public PlayerCharacteristics GetPlayerCharacteristics() => playerCharacteristics;
    public Vector3 GetPlayerPosition() => transform.position;
    public PlayerStepsSound GetPlayerStepsSound() => playerStepsSound;
    public PlayerFootstepVFX GetPlayerFootstepVFX() => playerFootstepVFX;
    public FishingRod GetFishingRod() => fishingRod;

    private bool ValidateDependencies(string caller)
    {
        if (movement == null) return LogMissingDependency(nameof(movement), caller);
        if (inventory == null) return LogMissingDependency(nameof(inventory), caller);
        if (playerCharacteristics == null) return LogMissingDependency(nameof(playerCharacteristics), caller);
        if (playerStepsSound == null) return LogMissingDependency(nameof(playerStepsSound), caller);
        if (playerFootstepVFX == null) return LogMissingDependency(nameof(playerFootstepVFX), caller);
        if (fishingRod == null) return LogMissingDependency(nameof(fishingRod), caller);

        return true;
    }

    private bool LogMissingDependency(string dependencyName, string caller)
    {
        Debug.LogError($"{nameof(PlayerController)}.{caller}: {dependencyName} is null on {name}.");
        return false;
    }
}