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

    [Header("Settings")]
    [Tooltip("How far from opponent should entity stop for them to not collide with each other")]
    [SerializeField] private float movementOffset = 1.5f;


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

    private void FixedUpdate()
    {
        NavigateTowardsPlayer();
        RotateModelTowardsPlayer();

        if (EnemySO != null)
        {
            if (attackCooldown.CooldownEnded && GetDistanceToPlayer() <= EnemySO.AttackRange)
            {
                AttackPlayer();
                attackCooldown.StartCooldown();
            }
        }
    }

    private void LateUpdate()
    {
        CopyPositionToModel();
        CopyRotationToModel();
    }


    private void NavigateTowardsPlayer()
    {
        Vector3 movePos = Vector3.MoveTowards(playerTransform.position, transform.position, movementOffset);
        navMesh.SetDestination(movePos);
    }
    private void RotateModelTowardsPlayer()
    {
        transform.LookAt(new Vector3(playerTransform.position.x, enemyModel.transform.position.y, playerTransform.position.z));
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, playerTransform.position);
    }

	private void CopyPositionToModel()
	{
		enemyModel.transform.position = transform.position;

	}
    private void CopyRotationToModel()
    {
        enemyModel.transform.rotation = transform.rotation;
    }

    private void AttackPlayer()
    {
        GameController.Instance.PlayerController.stats.TakeDamage(EnemySO.Damage);
    }


    private void OnDrawGizmos()
    {
        if (GameController.Instance != null && GameController.Instance.ShowDebug)
        {
            if (GameController.Instance.IsGameStarted)
            {
                if (GetDistanceToPlayer() <= EnemySO.AttackRange)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.yellow;
                }

                Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), transform.forward * EnemySO.AttackRange);
            }
        }
    }
}
