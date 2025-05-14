using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public CharacterDB mobDB;
    public List<WaveData> waveDB;
    public int nowWAVE = 0;
    public Transform[] spawnPos;

    public void SetWave()
    {
        nowWAVE = 1;
        WaveStart(waveDB[nowWAVE]);
    }

    public void WaveStart(WaveData wave)
    {
        for(int i = 0; i<wave.spawnEnemyData.Count; i++)
        {
            EnemySpawn(wave.spawnEnemyData[i]);
        }
    }

    public void EnemySpawn(DefaultDatable enemy)
    {
        int rnd = Random.Range(0, spawnPos.Length);
        Instantiate(waveDB[enemy.id], spawnPos[rnd].position, Quaternion.identity);
        
    }



}
