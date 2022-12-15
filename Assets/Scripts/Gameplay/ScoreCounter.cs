using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreCounter : MonoBehaviour
{
    public int Score = 0;
    public int CurrentMultiplier = 1;

    [Header("Events")]
    public UnityEvent OnScoreChange;


    private void Start()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.ScoreCounter = this;
        }
    }

    public void AddScore(int score)
    {
        Score += score * CurrentMultiplier;

        AudioController.Instance.Play("ScoreAdded");

        OnScoreChange.Invoke();
    }


    private void OnGUI()
    {
        if (GameController.Instance.ShowDebug)
        {
            GUI.color = Color.black;

            GUILayout.BeginArea(new Rect(0, 125f, 150f, 25f));

            GUILayout.Label($"Score: {Score}");

            GUILayout.EndArea();
        }
    }
}
