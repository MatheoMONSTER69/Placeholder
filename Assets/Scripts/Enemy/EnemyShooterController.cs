using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShooterController : EnemyController
{
    [Header("Projectile")]
    [SerializeField] private Transform projectileShooter;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 5.0f;
    [SerializeField] private float projectileDisableAfter = 5.0f;

    [SerializeField] private LayerMask projectileCollisionLayer;


    protected override void AttackPlayer()
    {
        anim.SetTrigger("Attack");

        GameController.Instance.AudioController.Play("EnemyAttack");

        ShootProjectile();
    }

    private void ShootProjectile()
    {
        GameObject projectileGO = Instantiate(projectilePrefab, projectileShooter.position, Quaternion.LookRotation(-projectileShooter.forward));

        Projectile projectile = projectileGO.GetComponent<Projectile>();

        projectile.Init(projectileSpeed, projectileCollisionLayer, ProjectileCollision, projectileDisableAfter, false, true);
    }

    private void ProjectileCollision(Vector3 position, GameObject col)
    {
        if(col.CompareTag("Player"))
        {
            GameController.Instance.PlayerController.stats.TakeDamage(EnemySO.Damage);
        }
    }
}
