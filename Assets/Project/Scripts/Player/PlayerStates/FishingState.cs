using UnityEngine;

public class FishingState : PlayerState
{
    [SerializeField] private FishingController fishingController;
    private PlayerMovement playerMovement;
    private FishingZone fishingZone;
    private FishingRod fishingRod;
    private bool startedFishing = false;
    public FishingState(PlayerController playerController, PlayerStateController stateController, FishingController fishingController) : base(playerController, stateController)
    {
        this.fishingController = fishingController;
    }

    public override void InitializeState()
    {
        fishingController.OnExit += ExitFishing;

        fishingZone = fishingController.GetFishingZone();
        playerMovement = playerController.GetPlayerMovement();
        fishingRod = playerController.GetFishingRod();
    }
    public override void OnEnter()
    {
        startedFishing = false;

        playerMovement.EnableJoystick(false);
        playerMovement.SetTargetPosition(fishingZone.GetPosition());
    }
    private void ExitFishing()
    {
        stateController.EnterState(stateController.idleState);
    }

    public override void OnExit()
    {
        playerController.GetPlayerMovement().EnableJoystick(true);

        fishingRod.Hide();
    }

    public override void OnUpdate()
    {
        if (startedFishing) return;

        if (playerMovement.IsBusy())
        {
            playerController.MovePlayer();
        }
        else
        {
            EnterFishing();

            playerMovement.transform.rotation = Quaternion.identity;
        }
    }
    private void EnterFishing()
    {
        startedFishing = true;

        fishingController.StartFishing();

        fishingRod.Show();

        playerController.EnterFishingAnim();
    }
}
