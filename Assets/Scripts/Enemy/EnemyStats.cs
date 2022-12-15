using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


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

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        AudioController.Instance.Play("EnemyTakeDamage");
    }

    public override void Die()
    {
        base.Die();

        AudioController.Instance.Play("EnemyDie");

        if (GameController.Instance != null)
        {
            GameController.Instance.ScoreCounter.AddScore(PointsForKill);

            if (GameController.Instance.WavesController != null)
            {
                GameController.Instance.EnemySpawner.RemoveEnemyFromCount(enemyType);
            }
        }

        EnemyRemainsPlacer.PlaceRemains(transform.position);

        Destroy(transform.parent.gameObject);
	}
}
