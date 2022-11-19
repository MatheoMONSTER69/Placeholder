using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class WavesController : MonoBehaviour
{
    [Header("Enemies")]
    public SerializableDictionary<EnemyType, List<EnemySO>> Enemies = new();
    [HideInInspector] public Dictionary<EnemyType, int> EnemyCount = new();

    [Header("Waves")]
    public List<Wave> Waves = new();
    public int CurrentWave = 0;

    public Timer WaveTimer = new();


    private void Start()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.WavesController = this;
        }

        ClearCounts();
    }

    private IEnumerator SpawnEnemies()
    {
        //TODO 
        yield return null;
    }

    private void ClearCounts()
    {
        EnemyCount.Clear();
        foreach (EnemyType type in (EnemyType[])Enum.GetValues(typeof(EnemyType)))
        {
            EnemyCount.Add(type, 0);
        }
    }

    public void AddEnemyToCount(EnemyType type)
    {
        EnemyCount[type]++;
    }
    public void RemoveEnemyFromCount(EnemyType type)
    {
        EnemyCount[type]--;
    }
}
