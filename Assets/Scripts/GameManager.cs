using UnityEngine;

public class GameManager : MonoSigleTone<GameManager>
{
    Spawner spawner;
    public GameObject playerobj;
    Transform playerObjTransform;
    CircleCollider2D userCollider;

    private void Start()
    {
        spawner = GetComponent<Spawner>();
        playerObjTransform = playerobj.GetComponent<Transform>();
        userCollider = playerobj.GetComponent<CircleCollider2D>();
    }

    public void biggerPlayer()
    {
        playerObjTransform.localScale = new Vector2(playerObjTransform.localScale.x * 1.1f, playerObjTransform.localScale.y * 1.1f);
        //userCollider.radius = 
        //콜라이더 크기 조절해주는 코드 짤 것
    }

    public void StateCheck()
    {
        spawner.checkState();
    }
}
