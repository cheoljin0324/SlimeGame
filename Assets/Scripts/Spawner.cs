using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

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

    private void Start()
    {
        SetWave();
    }

    private void Update()
    {
        if (enemyCnt == 0)
        {
            EndWave();
        }   
    }

    public void SetWave()
    {
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
        cntText.gameObject.SetActive(true);
        for(int i = 1; i<=enemyCnt; i++)
        {
            cntText.text = (enemyCnt - i).ToString();
            yield return new WaitForSeconds(10f);
        }
    }



}
