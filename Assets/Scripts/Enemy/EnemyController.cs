using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	[Header("References")]
	public EnemySO EnemySO = null;

	[SerializeField] protected Transform enemyModel;
    protected Animator anim;

	protected Transform playerTransform;
    protected NavMeshAgent navMesh;
    protected EnemyStats stats;
    protected Collider col;

    protected Cooldown attackCooldown;

    [Header("Settings")]
    [Tooltip("How far from opponent should entity stop for them to not collide with each other")]
    [SerializeField] protected float movementOffset = 1.5f;


    protected virtual void Start()
    {
        playerTransform = GameController.Instance.PlayerController.transform;
        navMesh = GetComponent<NavMeshAgent>();
        stats = GetComponent<EnemyStats>();
        anim = enemyModel.GetComponentInChildren<Animator>();
        col = GetComponent<Collider>();

        if (EnemySO != null)
        {
            stats.Init(EnemySO);
		}
        else
        {
            Debug.LogWarning($"{transform.name} has no EnemyScriptableObject assigned!");
        }

        navMesh.speed = EnemySO.MovingSpeed;

        float initialCooldown = Random.Range(0, (EnemySO.AttackCooldown / 2));
        attackCooldown = new(EnemySO.AttackCooldown + initialCooldown);
		attackCooldown.StartCooldown();

        stats.OnDamageTaken.AddListener(TakeDamage);
        stats.OnDeath.AddListener(Die);
    }

    protected virtual void OnDisable()
    {
        stats.OnDamageTaken.RemoveListener(TakeDamage);
        stats.OnDeath.RemoveListener(Die);
    }

    protected virtual void FixedUpdate()
    {
        if(!stats.IsDead)
        {
            NavigateTowardsPlayer();
            RotateModelTowardsPlayer();

            if (EnemySO != null)
            {
                if (!attackCooldown.IsInCooldown && GetDistanceToPlayer() <= EnemySO.AttackRange)
                {
                    AttackPlayer();
                    attackCooldown = new(EnemySO.AttackCooldown);
                    attackCooldown.StartCooldown();
                }
            }
        } 
    }

    private void LateUpdate()
    {
        CopyPositionToModel();
        CopyRotationToModel();
    }


    protected virtual void NavigateTowardsPlayer()
    {
        //Vector3 movePos = Vector3.MoveTowards(playerTransform.position, transform.position, movementOffset);

        Vector3 dir = (playerTransform.position - transform.position).normalized;

        Vector3 movePos = transform.position + dir;

        if (Vector3.Distance(playerTransform.position, transform.position) <= movementOffset * 0.9f)
        {
            movePos = transform.position - dir;
        }
        else if (Vector3.Distance(playerTransform.position, transform.position) <= movementOffset)
        {
            movePos = transform.position;
        }

        navMesh.SetDestination(movePos);
    }
    private void RotateModelTowardsPlayer()
    {
        transform.LookAt(new Vector3(playerTransform.position.x, enemyModel.transform.position.y, playerTransform.position.z));
    }

    protected float GetDistanceToPlayer()
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

    protected virtual void AttackPlayer()
    {
        anim.SetTrigger("Attack");

        GameController.Instance.AudioController.Play("EnemyAttack");

        GameController.Instance.PlayerController.stats.TakeDamage(EnemySO.Damage);
    }
    private void TakeDamage()
    {
        anim.SetTrigger("GetHurt");
        GameController.Instance.AudioController.Play("EnemyTakeDamage");
    }
    private void Die()
    {
        anim.SetTrigger("Dies");
        GameController.Instance.AudioController.Play("EnemyDie");

        navMesh.enabled = false;
        col.enabled = false;
    }


    protected virtual void OnDrawGizmos()
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
