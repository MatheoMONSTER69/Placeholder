using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	[Header("References")]
	public EnemySO EnemySO = null;

	[SerializeField] private Transform enemyModel;

	private Transform playerTransform;
    private NavMeshAgent navMesh;
    private EnemyStats stats;

    private Cooldown attackCooldown;


    private void Start()
    {
        playerTransform = GameController.Instance.PlayerController.transform;
        navMesh = GetComponent<NavMeshAgent>();
        stats = GetComponent<EnemyStats>();

        if(EnemySO != null)
        {
            stats.Init(EnemySO);
		}
        else
        {
            Debug.LogWarning($"{transform.name} has no EnemyScriptableObject assigned!");
        }

        navMesh.speed = EnemySO.MovingSpeed;

		attackCooldown = new(EnemySO.AttackCooldown);
		attackCooldown.StartCooldown();
	}
    
    private void Update()
    {
        NavigateTowardsPlayer();

        if(EnemySO != null)
        {
			if (attackCooldown.CooldownEnded && GetDistanceToPlayer() <= EnemySO.AttackRange)
			{
				AttackPlayer();
				attackCooldown.StartCooldown();
			}
		}
    }

    private void FixedUpdate()
    {
		CopyPositionToModel();
		RotateModelTowardsPlayer();
	}


    private void NavigateTowardsPlayer()
    {
        navMesh.SetDestination(playerTransform.position);
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, playerTransform.position);
    }

    private void AttackPlayer()
    {
		GameController.Instance.PlayerController.stats.TakeDamage(EnemySO.Damage);
	}

	private void CopyPositionToModel()
	{
		enemyModel.transform.position = transform.position;

	}
	private void RotateModelTowardsPlayer()
    {
        enemyModel.transform.LookAt(playerTransform.position);
    }
}
