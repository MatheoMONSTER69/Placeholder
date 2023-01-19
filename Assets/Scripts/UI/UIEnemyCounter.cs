using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIEnemyCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable()
    {
        Invoke(nameof(InvokedEnable), 0.1f);
    }
    private void InvokedEnable()
    {
        GameController.Instance.EnemySpawner.OnEnemySpawn.AddListener(UpdateText);
        GameController.Instance.EnemySpawner.OnEnemyRemove.AddListener(UpdateText);
    }

    private void OnDisable()
    {
        GameController.Instance.EnemySpawner.OnEnemySpawn.RemoveListener(UpdateText);
        GameController.Instance.EnemySpawner.OnEnemyRemove.RemoveListener(UpdateText);
    }

    private void UpdateText(EnemyType enemyType)
    {
        text.SetText(GameController.Instance.EnemySpawner.TotalCount.ToString());
    }
}
