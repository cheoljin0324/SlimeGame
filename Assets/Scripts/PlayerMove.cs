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
        // ���� �Է��� moveInput�� ���� �޾ƿ�
        Vector3 targetPosition = transform.position + new Vector3(moveInput.x, moveInput.y, 0f) * moveSpeed * Time.deltaTime;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothing);

        // ��׷��� ȿ��
        float stretch = moveInput.magnitude;
        Vector3 targetScale = new Vector3(
            originalScale.x + (stretch > 0 ? -squishAmount : 0),
            originalScale.y + (stretch > 0 ? squishAmount : 0),
            originalScale.z
        );

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * squishSpeed);
    }

    // Input System���� �� �޼��带 ȣ���ϰ� ���� (Input Action����)
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}


