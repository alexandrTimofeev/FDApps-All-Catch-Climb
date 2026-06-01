using UnityEngine;
using VContainer;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed = 1f;
    [Space(5)]
    [SerializeField] private float rotationSmoothness = 0.1f;
    private FloatingJoystick joystick;
    private CharacterController characterController;

    private Quaternion rotationOffset = Quaternion.Euler(0, -45, 0);
    private Vector2 moveDirection = Vector2.zero;
    private float currentAngle = 0f;
    private float angleVelocity;
    private Vector3 gravityVelocity = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;

    [Inject]
    public void Construct(FloatingJoystick joystick)
    {
        this.joystick = joystick;
    }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError($"{nameof(PlayerMovement)}: {nameof(characterController)} is null on {name}.");
            enabled = false;
            return;
        }

        transform.rotation = rotationOffset;
    }

    public void HandleInput()
    {
        if (joystick == null)
        {
            Debug.LogError($"{nameof(PlayerMovement)}.{nameof(HandleInput)}: {nameof(joystick)} is null on {name}.");
            moveDirection = Vector2.zero;
            return;
        }

        if (targetPosition == Vector3.zero)
        {
            moveDirection = joystick.Direction.normalized;
        }
        else
        {
            Vector3 toTarget = targetPosition - transform.position;
            toTarget.y = 0f;

            if (toTarget.magnitude < 0.1f || Vector3.Dot(transform.forward, toTarget.normalized) < 0f)
            {
                targetPosition = Vector3.zero;
                moveDirection = Vector2.zero;
            }
            else
            {
                moveDirection = new Vector2(toTarget.x, toTarget.z).normalized;
            }
        }
    }

    public void MovePlayer()
    {
        if (characterController == null)
        {
            Debug.LogError($"{nameof(PlayerMovement)}.{nameof(MovePlayer)}: {nameof(characterController)} is null on {name}.");
            return;
        }

        bool isMoving = moveDirection != Vector2.zero;

        if (isMoving)
        {
            HandleMovement();
            HandleRotation();
        }

        ApplyGravity();
    }

    private void HandleMovement()
    {
        Vector3 movementVector;

        if (targetPosition == Vector3.zero)
        {
            movementVector = rotationOffset * new Vector3(moveDirection.x, 0, moveDirection.y);
        }
        else
        {
            movementVector = new Vector3(moveDirection.x, 0, moveDirection.y);
        }

        characterController.Move(movementVector * moveSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        Vector3 targetDir;

        if (targetPosition == Vector3.zero)
            targetDir = rotationOffset * new Vector3(moveDirection.x, 0, moveDirection.y);
        else
            targetDir = new Vector3(moveDirection.x, 0, moveDirection.y);

        if (targetDir.sqrMagnitude > 0.001f)
        {
            float targetAngle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref angleVelocity, rotationSmoothness);
            transform.rotation = Quaternion.Euler(0, currentAngle, 0);
        }
    }



    private void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            gravityVelocity.y += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            gravityVelocity.y = -1f;
        }

        characterController.Move(gravityVelocity * Time.deltaTime);
    }

    public void EnableJoystick(bool b)
    {
        if (joystick == null)
        {
            Debug.LogError($"{nameof(PlayerMovement)}.{nameof(EnableJoystick)}: {nameof(joystick)} is null on {name}.");
            return;
        }

        joystick.gameObject.SetActive(b);

        if (!b)
        {
            joystick.ForcePointerUp();
        }
    }
    public void SetTargetPosition(Vector3 position) => targetPosition = position;
    public bool IsBusy() => targetPosition != Vector3.zero;

    public Vector2 GetInputVector() => moveDirection;
}
