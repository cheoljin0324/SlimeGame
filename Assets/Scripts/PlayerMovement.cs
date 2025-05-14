using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteAnimator))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Vector2 moveInput;
    private SpriteAnimator spriteAnimator;
    public PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
    }

    void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Disable();
    }

    void Start()
    {
        spriteAnimator = GetComponent<SpriteAnimator>();
    }

    void Update()
    {
        Vector3 moveDir = new Vector3(moveInput.x, moveInput.y, 0).normalized;
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);

        if (moveInput != Vector2.zero)
        {
            spriteAnimator.PlayMoveAnimation(moveInput);
        }
        else
        {
            spriteAnimator.PlayIdleAnimation();
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }
}
