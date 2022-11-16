using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public PlayerController PlayerController;
    //public WavesController WavesController;
    //public ScoreCounter ScoreCounter;

    [Header("Time")]
    public float CurrentTimeScale = 1.0f;
    private float previousTimeScale = 1.0f;
    public bool IsGamePaused => CurrentTimeScale == 0;

    [Header("Events")]
    public UnityEvent OnGamePause;
    public UnityEvent OnGameResume;


    #region Singleton
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion


    private void Start()
    {
        Time.timeScale = CurrentTimeScale;
        previousTimeScale = CurrentTimeScale;
    }

    public void PauseGame()
    {
        previousTimeScale = CurrentTimeScale;
        CurrentTimeScale = 0;
        Time.timeScale = CurrentTimeScale;

        OnGamePause.Invoke();
    }
    public void ResumeGame()
    {
        CurrentTimeScale = previousTimeScale;
        Time.timeScale = CurrentTimeScale;

        OnGameResume.Invoke();
    }
    public void TogglePauseGame()
    {
        if(IsGamePaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
}
