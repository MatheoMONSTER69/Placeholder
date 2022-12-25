using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossController : EnemyShooterController
{
    [SerializeField] private int burstCount = 5;
    [SerializeField] private float closeAttackDistance = 2.0f;
    [SerializeField] private float burstCooldown = 0.1f;


    protected override void AttackPlayer()
    {
        if (GetDistanceToPlayer() <= closeAttackDistance)
        {
            anim.SetTrigger("Attack");

            GameController.Instance.AudioController.Play("EnemyAttack");


            GameController.Instance.PlayerController.stats.TakeDamage(EnemySO.Damage);
        }
        else if (GetDistanceToPlayer() <= EnemySO.AttackRange)
        {
            anim.SetTrigger("Attack");

            GameController.Instance.AudioController.Play("EnemyAttack");


            StartCoroutine(BurstShootCoroutine(burstCount));
        }
    }

    private IEnumerator BurstShootCoroutine(int burstCount)
    {
        for (int i = 0; i < burstCount; i++)
        {
            ShootProjectile();

            yield return new WaitForSeconds(burstCooldown);
        }
    }
}
