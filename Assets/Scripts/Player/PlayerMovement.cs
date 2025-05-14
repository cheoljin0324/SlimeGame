using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Vector2 moveDirection;
    private SpriteRenderer spriteRenderer;
    private PlayerAnimation playerAnimation;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector2(horizontal, vertical).normalized;

        // ��ġ �̵�
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);

        // �ִϸ��̼� ���� ����
        if (moveDirection != Vector2.zero)
            playerAnimation.PlayAnimation(PlayerAnimation.State.Move);
        else
            playerAnimation.PlayAnimation(PlayerAnimation.State.Idle);

        // ���⿡ ���� �¿� ����
        if (horizontal != 0)
            spriteRenderer.flipX = horizontal > 0;
    }
}
