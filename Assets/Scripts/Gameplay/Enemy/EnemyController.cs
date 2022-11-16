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

    [Header("Settings")]
    private float enemySpeed = 5.0f;


    private void Start()
    {
        playerTransform = GameController.Instance.PlayerController.transform;
        navMesh = GetComponent<NavMeshAgent>();
        stats = GetComponent<EnemyStats>();

        if(EnemySO != null)
        {
            stats.Init(EnemySO);

            enemySpeed = EnemySO.MovingSpeed;
		}
        else
        {
            Debug.LogWarning($"{transform.name} has no EnemyScriptableObject assigned!");
        }

        navMesh.speed = enemySpeed;
	}
    
    private void Update()
    {
        NavigateTowardsPlayer();
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

	private void CopyPositionToModel()
	{
		enemyModel.transform.position = transform.position;

	}
	private void RotateModelTowardsPlayer()
    {
        enemyModel.transform.LookAt(playerTransform.position);
    }
}
