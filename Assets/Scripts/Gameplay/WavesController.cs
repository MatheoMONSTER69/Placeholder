using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEditor;
using System.Linq;
using static UnityEngine.EventSystems.EventTrigger;

public class WavesController : MonoBehaviour
{
    [Header("Waves")]
    [SerializeField] public List<Wave> Waves = new List<Wave>();
    public Wave CurrentWave => Waves.Count > 0 ? Waves[currentWaveId] : null;
    [HideInInspector] public int currentWaveId = 0;

    [Header("Settings")]
    [SerializeField] private float initialCooldown = 5;

    [Header("States")]
    [HideInInspector] public bool isWaveRunning = false;

    [Header("Timers")]
    [HideInInspector] public Timer waveTimer = new Timer();
    [HideInInspector] public CountdownTimer cooldownTimer = new CountdownTimer();

    [Header("Events")]
    public UnityEvent OnWaveStart;
    public UnityEvent OnWaveFinish;

    [Header("References")]
    private EnemySpawner enemySpawner;


    private void Start()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.WavesController = this;
        }

        currentWaveId = 0;
        isWaveRunning = false;

        //Start first wave
        StartCoroutine(WaveCooldown(0, initialCooldown));

        Invoke("AddSingletonListeners", 0.1f);
    }
    private void AddSingletonListeners()
    {
        enemySpawner = GameController.Instance.EnemySpawner;
        enemySpawner.OnNoEnemiesLeft.AddListener(FinishWave);
    }

    private void OnDisable()
    {
        if(enemySpawner != null)
        {
            enemySpawner.OnNoEnemiesLeft.RemoveListener(FinishWave);
        }
    }


    public void StartNextWave()
    {
        StartCoroutine(WaveCooldown((byte)(currentWaveId + 1), CurrentWave.CooldownAfterWave));
    }


    private void StartWave(byte waveId)
    {
        isWaveRunning = true;

        currentWaveId = waveId;

        waveTimer.StartTimer();

        StartCoroutine(enemySpawner.EnemySpawning(CurrentWave));

        GameController.Instance.AudioController.Play("WaveStart");

        OnWaveStart.Invoke();
    }

    public void FinishWave()
    {
        if (enemySpawner.allEnemiesSpawned == true)
        {
            if (Waves.Count <= currentWaveId + 1) //Finished all waves
            {
                isWaveRunning = false;
                GameController.Instance.StopGame(true);
            }
            else //Finish this wave, start next
            {
                DisableWave();

                OnWaveFinish.Invoke();

                //StartNextWave();
            }
        }
    }
    private void DisableWave()
    {
        isWaveRunning = false;

        waveTimer.StopTimer();

        CurrentWave.Finished = true;
        CurrentWave.Time = waveTimer.GetTime();

        GameController.Instance.AudioController.Play("WaveFinish");
    }
    
    public IEnumerator WaveCooldown(byte waveIdToLaunch, float time)
    {
        cooldownTimer.StartTimer(time);

        yield return new WaitForSeconds(time);

        cooldownTimer.StopTimer();

        StartWave(waveIdToLaunch);
    }

   


    private void OnGUI()
    {
        if (GameController.Instance.ShowDebug)
        {
            GUI.color = Color.black;

            GUILayout.BeginArea(new Rect(0, Screen.height - 250f, 250f, 250));

            GUILayout.Label($"CurrentWaveId: {currentWaveId}");
            GUILayout.Label($"IsWaveRunning: {isWaveRunning}");

            GUILayout.Label($"WaveTimer: {TimeConverter.ConvertTimeStripped(waveTimer.GetTime())}");
            GUILayout.Label($"cooldownTimer: {TimeConverter.ConvertTimeStripped(cooldownTimer.GetTime())}");

            GUILayout.Label($"maxEnemiesOnScene: {(enemySpawner != null ? enemySpawner.maxEnemiesOnScene : 0)}");
            GUILayout.Label($"AliveEnemiesCount: {(enemySpawner != null ? enemySpawner.TotalCount : 0)}");
            GUILayout.Label($"EnemiesSpawnedInCurrentWave: {(enemySpawner != null ? enemySpawner.enemiesSpawnedInCurrentWave : 0)}");
            GUILayout.Label($"CurrentWave.TotalEnemyCount: {CurrentWave.TotalEnemyCount}");
            GUILayout.Label($"cooldownBetweenEnemySpawn: {(enemySpawner != null ? enemySpawner.cooldownBetweenEnemySpawn : 0)}");
            GUILayout.Label($"AllEnemiesSpawned: {(enemySpawner != null ? enemySpawner.allEnemiesSpawned : false)}");

            GUILayout.EndArea();
        }
    }
}
