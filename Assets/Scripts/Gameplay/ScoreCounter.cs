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

        OnScoreChange.Invoke();
    }
}
