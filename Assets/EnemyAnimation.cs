using UnityEngine;
using System.Collections.Generic;

public class EnemyAnimation : MonoBehaviour
{
    //힘들다1123411456
    public enum State { Idle, Move }
    //테스트
    public List<Sprite> idleSprites;
    public List<Sprite> moveSprites;
    public float frameRate = 0.1f;

    private SpriteRenderer spriteRenderer;
    private float timer;
    private int currentFrame;
    private State currentState;
    private List<Sprite> currentSprites;
    private bool facingRight = true; // 기본적으로 오른쪽을 보고 있음

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        PlayAnimation(State.Idle);
    }

    void Update()
    {
        if (currentSprites == null || currentSprites.Count == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % currentSprites.Count;
            spriteRenderer.sprite = currentSprites[currentFrame];
        }
    }

    public void PlayAnimation(State newState)
    {
        if (newState == currentState) return;

        currentState = newState;
        currentFrame = 0;
        timer = 0f;

        currentSprites = (currentState == State.Move) ? moveSprites : idleSprites;
        spriteRenderer.sprite = currentSprites[0];
    }

    public void FlipSprite(bool flip)
    {
        if (flip != facingRight)
        {
            facingRight = flip;
            Vector3 theScale = transform.localScale;

            // 반전 방향을 반대로 설정
            theScale.x = Mathf.Abs(theScale.x) * (flip ? -1 : 1); // flip이 true이면 1, false이면 -1
            transform.localScale = theScale;
        }
    }

}
