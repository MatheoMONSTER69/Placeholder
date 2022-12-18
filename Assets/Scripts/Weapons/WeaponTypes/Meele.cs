using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Meele : Weapon
{
    [Header("Meele")]
    [SerializeField] private float range = 3.0f;
    [SerializeField] private float damageTime = 0.75f;

    [Header("Effects")]
    [Header("Bullet Trail")]
    [SerializeField] private TrailRenderer trail;
    /*[SerializeField] private float trailDuration = 0.19f;

    private Coroutine trailCoroutine;
    private bool trailCoroutineIsRunning = false;*/


    protected override void Start()
    {
        base.Start();
    }

    private void Awake()
    {
        //Effects
        /*if (trail != null)
        {
            trail.gameObject.SetActive(false);
        }*/
    }


    public override void Attack(Vector3 targetPos)
    {
        List<EnemyStats> enemies = GetEnemies(targetPos);


        //TrailEffect();


        ApplyDamage(enemies);


        base.Attack(targetPos);
    }


    protected override List<EnemyStats> GetEnemies(Vector3 targetPos)
    {
        List<EnemyStats> enemies = new();

        if (weaponBarrelEnd != null && playerController != null)
        {
            RaycastHit[] hits = ExtendedPhysics.OrderedSphereCastAll(playerController.transform.position, range / 2, playerController.transform.forward, range, CollisionLayer);

            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.TryGetComponent(out EnemyStats enemyStats))
                    {
                        enemies.Add(enemyStats);
                    }
                }
            }
        }

        return enemies;
    }

    protected override void ApplyDamage(List<EnemyStats> enemies)
    {
        if (enemies.Count > 0)
        {
            int damagedEnemies = 0;
            foreach (EnemyStats enemy in enemies)
            {
                //Apply half of weapon damage, then apply the other half scaled by how close the enemy is to the player
                float enemyFract = 1 - (float)((float)damagedEnemies / (float)enemies.Count);
                float delay = damageTime * enemyFract;

                StartCoroutine(DeleyedDamage(enemy, Damage, delay));

                damagedEnemies++;
            }
        }
    }

    private IEnumerator DeleyedDamage(EnemyStats enemy, float Damage, float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        if(enemy != null)
        {
            enemy.TakeDamage(Damage);
        }
    }


    //Effects
    /*private void TrailEffect()
    {
        if (trail != null)
        {
            if (trailCoroutineIsRunning)
            {
                StopCoroutine(trailCoroutine);
                trailCoroutineIsRunning = false;
            }

            trailCoroutine = StartCoroutine(Trail(trailDuration));
        }
    }
    private IEnumerator Trail(float trailDuration)
    {
        trailCoroutineIsRunning = true;

        yield return new WaitForSeconds(trailDuration);

        trail.gameObject.SetActive(false);

        trailCoroutineIsRunning = false;
    }*/


    protected override void OnGUI()
    {
        if (GameController.Instance.ShowDebug)
        {
            GUI.color = Color.black;

            GUILayout.BeginArea(new Rect(Screen.width - 250f, (Screen.height / 2) + 100f, 250f, 150f));

            GUILayout.Label($"Name: {Name}");
            GUILayout.Label($"Damage: {Damage}");
            GUILayout.Label($"AttackSpeed: {AttackSpeed}");
            GUILayout.Label($"AttackCooldown: {AttackCooldown.IsInCooldown}");
            GUILayout.Label($"Range: {range}");
            GUILayout.Label($"DamageTime: {damageTime}");

            GUILayout.EndArea();
        }
    }
}
