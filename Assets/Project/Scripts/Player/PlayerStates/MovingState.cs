using UnityEngine;

public class MovingState : PlayerState
{
    private PlayerStepsSound playerStepsSound;
    private PlayerFootstepVFX playerFootstepVFX;

    public MovingState(PlayerController playerController, PlayerStateController stateController)
        : base(playerController, stateController)
    {
    }

    public override void InitializeState()
    {
        playerStepsSound = playerController.GetPlayerStepsSound();
        playerFootstepVFX = playerController.GetPlayerFootstepVFX();
    }

    public override void OnEnter()
    {
        playerController.EnterMovingAnim();

        playerStepsSound.StartPlaying();
        playerFootstepVFX.PlayVFX();
    }

    public override void OnExit()
    {
        playerStepsSound.StopPlaying();
        playerFootstepVFX.StopVFX();
    }

    public override void OnUpdate()
    {
        playerController.MovePlayer();

        if (playerController.GetPlayerMovement().GetInputVector() == Vector2.zero)
        {
            stateController.EnterState(stateController.idleState);
        }
    }
}