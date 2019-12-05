using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<WaveConfig> waveConfigs;
    [SerializeField] int startingWave = 0;
    [SerializeField] bool looping = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // if there are no waves configured do not attempt to spawn waves
        if (waveConfigs.Count <= 0) yield break;

        do
        {
            yield return StartCoroutine(SpawnAllWaves());
        }
        while (looping);
    }

    /// <summary>
    /// Spawns all the enemy waves one after the other one, nested coroutines
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnAllWaves()
    {
        for(int waveCount = startingWave; waveCount < waveConfigs.Count; waveCount++)
        {
            yield return StartCoroutine(SpawnAllEnemiesInWave(waveConfigs[waveCount]));
        }
    }

    /// <summary>
    /// Spawns a single enemy wave based on a waveConfig
    /// </summary>
    /// <param name="waveConfig"></param>
    /// <returns></returns>
    private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig)
    {
        for(int enemyCount = 0; enemyCount < waveConfig.GetNumberOfEnemies(); enemyCount++)
        {
            var newEnemy = Instantiate(
                waveConfig.GetEnemyPrefab(), 
                waveConfig.GetWayPoints()[0].transform.position, 
                Quaternion.identity);

            EnemyPathing enemyPathing = newEnemy.GetComponent<EnemyPathing>();
            enemyPathing.SetWaveConfig(waveConfig);

            yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawns());
        }
    }

}
