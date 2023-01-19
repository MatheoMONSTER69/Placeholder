using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-50)]
public class GameController : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public PlayerController PlayerController;
    [HideInInspector] public WavesController WavesController;
    [HideInInspector] public EnemySpawner EnemySpawner;
    [HideInInspector] public ScoreCounter ScoreCounter;
    [HideInInspector] public AudioController AudioController;
    [HideInInspector] public WeaponController WeaponController;

    private InputAction pauseGameplay;
	private InputAction pauseMenu;

	[Header("Time")]
    public float CurrentTimeScale = 1.0f;
    private float previousTimeScale = 1.0f;
    public bool IsGamePaused => CurrentTimeScale == 0;
    public bool IsGameStarted = false;

	public Timer GameTimer = new();

    [Header("Events")]
	public UnityEvent OnGameStart;
	public UnityEvent OnGamePause;
    public UnityEvent OnGameResume;
	public UnityEvent<bool> OnGameStop;

    [Header("Debug")]
    public bool ShowDebug = true;


    public static GameController Instance { get; private set; }

    private void Awake()
    {
        #region Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        #endregion


        pauseGameplay = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.PauseToggle);
		pauseMenu = InputManager.Instance.GetAction(ActionMapType.Menu, InputType.PauseToggle);

	}

    private void Start()
    {
        StartGame(); //TEMP
    }

    private void Update()
    {
        if(pauseMenu.triggered || pauseGameplay.triggered)
        {
            TogglePauseGame();
        }
    }


    private void StartGame()
    {
        InputManager.Instance.SwitchCurrentActionMap(ActionMapType.Gameplay);

        Time.timeScale = CurrentTimeScale;
        previousTimeScale = CurrentTimeScale;

        GameTimer.StartTimer();

		IsGameStarted = true;

		OnGameStart.Invoke();
	}

    public void PauseGame()
    {
        InputManager.Instance.SwitchCurrentActionMap(ActionMapType.Menu);

        previousTimeScale = CurrentTimeScale;
        CurrentTimeScale = 0;
        Time.timeScale = CurrentTimeScale;

        GameTimer.PauseTimer();

        Debug.Log("Paused the game");

        OnGamePause.Invoke();

        SceneManager.LoadSceneAsync(0); //TEMP
    }

    public void ResumeGame()
    {
        InputManager.Instance.SwitchCurrentActionMap(ActionMapType.Gameplay);

        CurrentTimeScale = previousTimeScale;
        Time.timeScale = CurrentTimeScale;

        GameTimer.ResumeTimer();

        Debug.Log("Resumed the game");

        OnGameResume.Invoke();
    }

	public void StopGame(bool win)
	{
		InputManager.Instance.SwitchCurrentActionMap(ActionMapType.Menu);

		CurrentTimeScale = 0;
		Time.timeScale = CurrentTimeScale;

		GameTimer.StopTimer();

        IsGameStarted = false;

        Debug.Log($"{(win ? "Won" : "Lost")} the game");

        OnGameStop.Invoke(win);

        SceneManager.LoadSceneAsync(0); //TEMP
    }

	[ContextMenu("Toggle Pause")]
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


    private void OnGUI()
    {
        if (ShowDebug)
        {
            GUI.color = Color.black;

            GUILayout.BeginArea(new Rect(0, 0, 150f, 100f));

            GUILayout.Label($"IsGamePaused: {IsGamePaused}");
            GUILayout.Label($"IsGameStarted: {IsGameStarted}");

            GUILayout.Label($"TimeScale: {Time.timeScale}");

            GUILayout.Label($"GameTimer: {TimeConverter.ConvertTimeStripped(GameTimer.GetTime())}");

            GUILayout.EndArea();
        }
    }
}
