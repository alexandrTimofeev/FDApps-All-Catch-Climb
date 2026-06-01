using UnityEngine;
using VContainer;

public class PlayerStateController : MonoBehaviour
{
    public IdleState idleState;
    public MovingState movingState;
    public FishingState fishingState;
    private PlayerController playerController;
    private FishingController fishingController;

    private PlayerState currentState;
    private bool isInitialized;

    [Inject]
    public void Construct(PlayerController playerController, FishingController fishingController)
    {
        this.playerController = playerController;
        this.fishingController = fishingController;
    }

    void Start()
    {
        InitializeStateMachine();
    }

    private void InitializeStateMachine()
    {
        if (isInitialized)
            return;

        if (playerController == null)
        {
            Debug.LogError($"{nameof(PlayerStateController)}: {nameof(playerController)} is null on {name}.");
            return;
        }

        if (fishingController == null)
        {
            Debug.LogError($"{nameof(PlayerStateController)}: {nameof(fishingController)} is null on {name}.");
            return;
        }

        idleState = new IdleState(playerController, this);
        idleState.InitializeState();

        movingState = new MovingState(playerController, this);
        movingState.InitializeState();

        fishingState = new FishingState(playerController, this, fishingController);
        fishingState.InitializeState();

        EnterState(idleState);

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized || currentState == null)
            return;

        currentState.OnUpdate();
    }
    public void EnterState(PlayerState newState)
    {
        if (newState == null)
        {
            Debug.LogError($"{nameof(PlayerStateController)}.{nameof(EnterState)}: {nameof(newState)} is null on {name}.");
            return;
        }

        currentState?.OnExit();

        currentState = newState;

        newState.OnEnter();
    }
}
