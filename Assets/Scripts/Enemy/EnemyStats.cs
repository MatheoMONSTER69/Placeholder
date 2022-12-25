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

    [SerializeField] private Transform enemyModel;

    [Header("Settings")]
    [SerializeField] private float destroyAfter = 3.0f;
    [SerializeField] private float sizeChangeSpeed = 0.5f;


    public void Init(EnemySO enemySO)
    {
        enemyType = enemySO.EnemyType;
        MaxHealth = enemySO.Health;
        Damage = enemySO.Damage;
        PointsForKill = enemySO.PointsForKill;

        Health = MaxHealth;

        StartCoroutine(ShowObject(enemyModel.localScale, sizeChangeSpeed));
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

        EnemyParticlesPlacer.SpawnParticles(transform.position);

        StartCoroutine(DestroyObject(destroyAfter, sizeChangeSpeed));
	}


    private IEnumerator ShowObject(Vector3 targetScale, float speed)
    {
        float t = 0;
        float time = 0;

        while (t < 1)
        {
            t = time / speed;

            float scale = Mathf.Lerp(0, targetScale.x, t);
            enemyModel.localScale = new Vector3(scale, scale, scale);

            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        enemyModel.localScale = targetScale;
    }

    private IEnumerator DestroyObject(float after, float shrinkSpeed)
    {
        yield return new WaitForSeconds(after);

        float t = 0;
        float time = 0;
        float startScale = enemyModel.localScale.x;

        while (t < 1)
        {
            t = time / shrinkSpeed;

            float scale = Mathf.Lerp(startScale, 0, t);
            enemyModel.localScale = new Vector3(scale, scale, scale);

            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        Destroy(transform.parent.gameObject);
    }
}
