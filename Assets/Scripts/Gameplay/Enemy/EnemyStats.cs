using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    private int PointsForKill = 1;

	public void Init(EnemySO enemySO)
    {
        MaxHealth = enemySO.Health;
        Damage = enemySO.Damage;
        PointsForKill = enemySO.PointsForKill;
	}

    public override void Die()
    {
        base.Die();

        //GameController.Instance.ScoreCounter.AddScore(PointsForKill);
	}
}
