using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    [Header("Enemy")]
    public float Damage = 5;
    private int PointsForKill = 1;
    private EnemyType enemyType;


	public void Init(EnemySO enemySO)
    {
        enemyType = enemySO.EnemyType;
        MaxHealth = enemySO.Health;
        Damage = enemySO.Damage;
        PointsForKill = enemySO.PointsForKill;

        Health = MaxHealth;
	}

    public override void Die()
    {
        base.Die();

        if (GameController.Instance != null)
        {
            GameController.Instance.ScoreCounter.AddScore(PointsForKill);

            if (GameController.Instance.WavesController != null)
            {
                GameController.Instance.EnemySpawner.RemoveEnemyFromCount(enemyType);
            }
        }

        Destroy(transform.parent.gameObject);
	}
}
