using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public enum spawnState
{
    None, Start, Game, Cull,End,GameEnd
}

public class Spawner : MonoBehaviour
{
    public CharacterDB mobDB;
    public List<WaveData> waveDB;
    public int nowWAVE = 0;
    public Transform[] spawnPos;
    public List<GameObject> mobData;
    public int enemyCnt = 0;
    public TextMeshProUGUI cntText;
    public bool gameStart = false;
    public spawnState gameState = spawnState.None;

    private void Start()
    {
        SetWave();
    }

    private void Update()
    {
        if (enemyCnt == 0 && gameState == spawnState.End)
        {
            gameState = spawnState.Cull;
            EndWave();
        }   
    }

    public void checkState()
    {
        if (enemyCnt == 0)
        {
            gameState = spawnState.End;
        }
    }

    public void SetWave()
    {
        gameState = spawnState.Start;
        nowWAVE = 0;
        WaveStart(waveDB[nowWAVE]);
        gameStart = true;
    }

    public void WaveStart(WaveData wave)
    {
        nowWAVE++;
        for(int i = 0; i<wave.spawnEnemyData.Count; i++)
        {
            EnemySpawn(wave.spawnEnemyData[i]);
            enemyCnt++;
        }
        gameState = spawnState.Game;
    }

    public void EnemySpawn(DefaultDatable enemy)
    {
        int rnd = Random.Range(0, spawnPos.Length-1);
        Instantiate(mobDB.charDB[enemy.id], spawnPos[rnd].position, Quaternion.identity);
    }

    void EndWave()
    {
        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        int cntCull = 10;
        cntText.gameObject.SetActive(true);
        for(int i = cntCull; i>=0; i--)
        {
            Debug.Log(cntCull);
            Debug.Log(i);
            cntText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        if (nowWAVE == waveDB.Count) gameState = spawnState.GameEnd;
        else { 
            gameState = spawnState.Start;
            WaveStart(waveDB[nowWAVE]);
        }

        cntText.gameObject.SetActive(false);
    }



}
