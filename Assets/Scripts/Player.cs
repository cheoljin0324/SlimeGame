using UnityEngine;

public enum playerState
{
    None, Idle, Move, Attack, Damage, Death
}

public enum animState
{
    None, Idle, Move, Attack, Damage, Death
}

public class Player : MonoBehaviour
{
    PlayerAnim charAnimation;
    PlayerMove useMove;

    private void Start()
    {
        charAnimation = GetComponentInChildren<PlayerAnim>();
        useMove = GetComponentInChildren<PlayerMove>();
    }

    private void Update()
    {
        
    }

    public animState GetAnim()
    {
        return charAnimation.nowState;
    }

    public void AnimIdle()
    {
        charAnimation.nowState = animState.Idle;

        StopAllCoroutines();
        StartCoroutine(charAnimation.charIdleMotion());
    }

    public void AnimMove()
    {
        charAnimation.nowState = animState.Move;

        StopAllCoroutines();
        StartCoroutine(charAnimation.MoveAnim());
    }


}
