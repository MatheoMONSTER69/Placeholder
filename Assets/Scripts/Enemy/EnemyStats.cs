using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    [Header("Enemy")]
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

        GameController.Instance.ScoreCounter.AddScore(PointsForKill);

        if(GameController.Instance != null && GameController.Instance.WavesController != null)
        {
            GameController.Instance.WavesController.RemoveEnemyFromCount(enemyType);
        }
	}
}
