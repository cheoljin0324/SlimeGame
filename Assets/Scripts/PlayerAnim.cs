using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    public List<Sprite> playerIdleSprite;
    public List<Sprite> playerMoveSprite;
    int Motionindex = 0;
    SpriteRenderer playerSpriteRenderer;
    animState nowState = animState.None;

    private void Start()
    {
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        nowState = animState.Idle;
        StartCoroutine(charIdleMotion());
    }

    void SetAnim()
    {
        Motionindex = 0;
    }

    public IEnumerator MoveAnim()
    {
        SetAnim();
        while (true)
        {
            if (Motionindex > playerMoveSprite.Count - 2)
            {
                Motionindex = 0;
            }
            Motionindex++;
            playerSpriteRenderer.sprite = playerMoveSprite[Motionindex];
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator charIdleMotion()
    {
        SetAnim();
        while (true)
        {
            if (Motionindex > playerIdleSprite.Count - 2)
            {
                Motionindex = 0;
            }
            Motionindex++;
            playerSpriteRenderer.sprite =  playerIdleSprite[Motionindex];
            yield return new WaitForSeconds(0.1f);
        }
    }
}
