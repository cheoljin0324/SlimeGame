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

        // 위치 이동
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);

        // 애니메이션 상태 결정
        if (moveDirection != Vector2.zero)
            playerAnimation.PlayAnimation(PlayerAnimation.State.Move);
        else
            playerAnimation.PlayAnimation(PlayerAnimation.State.Idle);

        // 방향에 따라 좌우 반전
        if (horizontal != 0)
            spriteRenderer.flipX = horizontal > 0;
    }
}
