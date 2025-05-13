using UnityEngine;

enum playerState
{
    None, Idle, Move, Attack, Damage, Death
}

enum animState
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
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) AnimMove();
    }

    public void AnimMove()
    {
        StopCoroutine(charAnimation.charIdleMotion());
        StartCoroutine(charAnimation.MoveAnim());
    }


}
