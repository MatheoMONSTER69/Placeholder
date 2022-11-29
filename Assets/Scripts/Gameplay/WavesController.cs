using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEditor;
using System.Linq;

public class WavesController : MonoBehaviour
{
    [Header("Waves")]
    [SerializeField] public List<Wave> Waves = new List<Wave>();

    [SerializeField] private float initialCooldown = 5;

    [HideInInspector] public bool isWaveRunning = false;

    [HideInInspector] public int currentWaveId = 0;

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

        enemySpawner = GameController.Instance.EnemySpawner;
        enemySpawner.OnNoEnemiesLeft.AddListener(FinishWave);

        currentWaveId = 0;

        isWaveRunning = false;

        //Start first wave
        StartCoroutine(WaveCooldown(0, initialCooldown));
    }

    private void OnDisable()
    {
        if(enemySpawner != null)
        {
            enemySpawner.OnNoEnemiesLeft.RemoveListener(FinishWave);
        }
    }


    private void StartWave(byte waveId)
    {
        isWaveRunning = true;

        currentWaveId = waveId;

        waveTimer.StartTimer();

        //Spawn entities
        Wave wave = Waves[waveId];
        foreach (EnemySO enemy in wave.Enemies.dictionary.Keys)
        {
            for (int i = 0; i < wave.Enemies.dictionary[enemy]; i++)
            {
                enemySpawner.SpawnEnemy(enemy);
            }
        }

        OnWaveStart.Invoke();
    }

    private void DisableWave()
    {
        isWaveRunning = false;

        waveTimer.StopTimer();

        Waves[currentWaveId].Finished = true;
        Waves[currentWaveId].Time = waveTimer.GetTime();
    }

    public void FinishWave()
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

            StartCoroutine(WaveCooldown((byte)(currentWaveId + 1), Waves[currentWaveId].Cooldown));
        }
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

            GUILayout.BeginArea(new Rect(0, Screen.height - 100f, 150f, 100));

            GUILayout.Label($"CurrentWaveId: {currentWaveId}");
            GUILayout.Label($"IsWaveRunning: {isWaveRunning}");

            GUILayout.Label($"WaveTimer: {TimeConverter.ConvertTimeStripped(waveTimer.GetTime())}");

            GUILayout.Label($"EnemiesCount: {enemySpawner.TotalCount}");

            GUILayout.EndArea();
        }
    }
}
