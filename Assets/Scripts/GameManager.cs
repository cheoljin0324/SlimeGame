using UnityEngine;

public class GameManager : MonoSigleTone<GameManager>
{
    Spawner spawner;

    private void Start()
    {
        spawner = GetComponent<Spawner>();
    }
    public void StateCheck()
    {
        spawner.checkState();
    }
}
