using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private bool spawningEnabled = true;

    public float maxEnemiesOnScene = 100;
    public float cooldownBetweenEnemySpawn = 0.1f;

    [Space(15)]

    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField] private Transform enemyContainer;

    [HideInInspector] public Dictionary<EnemyType, int> EnemyCount = new();
    public int TotalCount => EnemyCount.Values.Sum();

    [HideInInspector] public bool allEnemiesSpawned = false;
    [HideInInspector] public int enemiesSpawnedInCurrentWave = 0;

    [Header("Events")]
    public UnityEvent<EnemyType> OnEnemySpawn;
    public UnityEvent<EnemyType> OnEnemyRemove;
    public UnityEvent OnNoEnemiesLeft;



    private void Awake()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.EnemySpawner = this;
        }

        allEnemiesSpawned = false;
        enemiesSpawnedInCurrentWave = 0;

        ClearCounts();
    }


    public void SpawnEnemy(EnemySO enemy)
    {
        if(spawnPoints.Count > 0 && enemyContainer != null && spawningEnabled)
        {
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];

            Instantiate(enemy.Prefab, spawnPoint.position, spawnPoint.rotation, enemyContainer);

            AddEnemyToCount(enemy.EnemyType);

            GameController.Instance.AudioController.Play("EnemySpawn");
        }
        else
        {
            OnNoEnemiesLeft.Invoke();
        }
    }

    public void AddEnemyToCount(EnemyType type)
    {
        EnemyCount[type]++;

        OnEnemySpawn.Invoke(type);
    }
    public void RemoveEnemyFromCount(EnemyType type)
    {
        EnemyCount[type]--;

        OnEnemyRemove.Invoke(type);

        if(TotalCount <= 0)
        {
            OnNoEnemiesLeft.Invoke();
        }
    }

    private void ClearCounts()
    {
        EnemyCount.Clear();
        foreach (EnemyType type in (EnemyType[])Enum.GetValues(typeof(EnemyType)))
        {
            EnemyCount.Add(type, 0);
        }
    }


    public IEnumerator EnemySpawning(Wave CurrentWave)
    {
        allEnemiesSpawned = false;
        enemiesSpawnedInCurrentWave = 0;


        //Highest amount of enemeies under single type
        float maxEnemyAmountToSpawn = CurrentWave.Enemies.dictionary.Values.Max();

        //Assign default values
        Dictionary<EnemySO, int> waitCyclesToSpawn = new();
        Dictionary<EnemySO, int> enemiesSpawned = new();

        foreach (EnemySO enemy in CurrentWave.Enemies.dictionary.Keys)
        {
            //wait spawn cycles before spawning this type of enemy
            float enemyAmountToSpawn = CurrentWave.Enemies.dictionary[enemy];

            float spawnEvery = maxEnemyAmountToSpawn / enemyAmountToSpawn;

            waitCyclesToSpawn.Add(enemy, (int)spawnEvery);

            //Enemies of type spawned counter
            enemiesSpawned.Add(enemy, 0);
        }

        //Cycle through enemies and spawn them
        while (enemiesSpawnedInCurrentWave < CurrentWave.TotalEnemyCount)
        {
            while (TotalCount < maxEnemiesOnScene)
            {
                foreach (EnemySO enemy in CurrentWave.Enemies.dictionary.Keys)
                {
                    int enemyAmountToSpawn = CurrentWave.Enemies.dictionary[enemy];

                    //if there should be more enemies of this type spawned
                    if (enemiesSpawned[enemy] < enemyAmountToSpawn)
                    {
                        waitCyclesToSpawn[enemy]--;

                        if (waitCyclesToSpawn[enemy] == 0)
                        {
                            //Spawn enemy
                            SpawnEnemy(enemy);

                            enemiesSpawned[enemy]++;
                            enemiesSpawnedInCurrentWave++;


                            //Reset waitCycles value
                            float spawnEvery = maxEnemyAmountToSpawn / enemyAmountToSpawn;
                            waitCyclesToSpawn[enemy] = (int)spawnEvery;
                        }
                    }

                    //All enemies got spawned
                    if (enemiesSpawnedInCurrentWave >= CurrentWave.TotalEnemyCount)
                    {
                        allEnemiesSpawned = true;
                        yield break;
                    }
                }

                yield return new WaitForSeconds(cooldownBetweenEnemySpawn);
            }

            yield return new WaitUntil(() => TotalCount < maxEnemiesOnScene * 0.5f);
        }
    }
}
