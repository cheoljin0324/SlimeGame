using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public Player player;

    public float moveSpeed = 3f;
    public float smoothing = 0.1f;
    public float squishAmount = 0.1f;
    public float squishSpeed = 4f;

    private Vector2 moveInput;
    private Vector3 velocity = Vector3.zero;
    private Vector3 originalScale;

    void Start()
    {
        player = GetComponent<Player>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        // 방향 입력은 moveInput을 통해 받아옴
        Vector3 targetPosition = transform.position + new Vector3(moveInput.x, moveInput.y, 0f) * moveSpeed * Time.deltaTime;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothing);

        // 찌그러짐 효과
        float stretch = moveInput.magnitude;
        Vector3 targetScale = new Vector3(
            originalScale.x + (stretch > 0 ? -squishAmount : 0),
            originalScale.y + (stretch > 0 ? squishAmount : 0),
            originalScale.z
        );

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * squishSpeed);
    }

    // Input System에서 이 메서드를 호출하게 설정 (Input Action에서)
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}


