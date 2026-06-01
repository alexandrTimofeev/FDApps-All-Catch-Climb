using UnityEngine;
using VContainer;

public class FishingZone : EventZone
{
    [SerializeField] private PlayerStateController stateController;
    [SerializeField] private PlayerInventory playerInventory;

    [Inject]
    public void Construct(PlayerStateController stateController, PlayerInventory playerInventory)
    {
        this.playerInventory = playerInventory;
    }

    public override void PlayerEnter()
    {
        if (stateController == null)
        {
            Debug.LogError($"{nameof(FishingZone)}.{nameof(PlayerEnter)}: {nameof(stateController)} is null on {name}.");
            return;
        }

        if (playerInventory == null)
        {
            Debug.LogError($"{nameof(FishingZone)}.{nameof(PlayerEnter)}: {nameof(playerInventory)} is null on {name}.");
            return;
        }

        if (!playerInventory.HasSpace())
        {
            UIWarningPopup.Instance.ShowWarning("You have no space left in inventory!", 4f);
            return;
        }
        stateController.EnterState(stateController.fishingState);
    }
    public Vector3 GetPosition() => transform.position;
}
