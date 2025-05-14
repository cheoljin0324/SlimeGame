using UnityEngine;
using System.Collections.Generic;

public class SpriteAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Header("Idle Animation")]
    public List<Sprite> idleSprites;

    [Header("Move Animation")]
    public List<Sprite> moveSprites; // �ϳ��� ���

    public float frameRate = 0.1f;

    private float timer;
    private int currentFrame;
    private List<Sprite> currentAnimation;

    private Vector2 lastDirection = Vector2.down;
    private bool isMoving;

    void Update()
    {
        if (currentAnimation == null || currentAnimation.Count == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % currentAnimation.Count;
            spriteRenderer.sprite = currentAnimation[currentFrame];
        }
    }

    public void PlayMoveAnimation(Vector2 direction)
    {
        isMoving = true;
        lastDirection = direction;

        currentAnimation = moveSprites;

        // Flip ó��: x �������� �̵� �ø� ����
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            spriteRenderer.flipX = direction.x < 0;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        ResetAnimation();
    }

    public void PlayIdleAnimation()
    {
        if (!isMoving) return;

        isMoving = false;
        currentAnimation = idleSprites;

        // ������ ���� ����ؼ� flipX ����
        if (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
        {
            spriteRenderer.flipX = lastDirection.x < 0;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        ResetAnimation();
    }

    private void ResetAnimation()
    {
        timer = 0f;
        currentFrame = 0;

        if (currentAnimation != null && currentAnimation.Count > 0)
        {
            spriteRenderer.sprite = currentAnimation[0];
        }
    }
}
