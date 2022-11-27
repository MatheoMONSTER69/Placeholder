using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private bool spawningEnabled = true;

    [Space(15)]

    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField] private Transform enemyContainer;

    [HideInInspector] public Dictionary<EnemyType, int> EnemyCount = new();
    public int TotalCount => EnemyCount.Values.Sum();

    [Header("Events")]
    public UnityEvent<EnemyType> OnEnemySpawn;
    public UnityEvent<EnemyType> OnEnemyRemove;
    public UnityEvent OnNoEnemiesLeft;



    private void Start()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.EnemySpawner = this;
        }

        ClearCounts();
    }


    public void SpawnEnemy(EnemySO enemy)
    {
        if(spawnPoints.Count > 0 && enemyContainer != null && spawningEnabled)
        {
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];

            Instantiate(enemy.Prefab, spawnPoint.position, spawnPoint.rotation, enemyContainer);

            AddEnemyToCount(enemy.EnemyType);
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
}
