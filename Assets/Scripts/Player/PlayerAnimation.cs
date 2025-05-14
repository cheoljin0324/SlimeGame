using UnityEngine;
using System.Collections.Generic;

public class PlayerAnimation : MonoBehaviour
{
    public enum State { Idle, Move }

    public List<Sprite> idleSprites;
    public List<Sprite> moveSprites;
    public float frameRate = 0.1f;

    private SpriteRenderer spriteRenderer;
    private float timer;
    private int currentFrame;
    private State currentState;
    private List<Sprite> currentSprites;

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
}
