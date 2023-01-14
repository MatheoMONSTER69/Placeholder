using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class ProjectileWeapon : Weapon
{
    [Header("ProjectileBasedWeapon")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 20.0f;
    [SerializeField] private float projectileDetonateRange = 4.0f;
    [SerializeField] private float projectileDetonateAfter = 3.0f;

    [Header("Effects")]
    [Header("Muzzle Flash")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private float muzzleFlashDuration = 0.05f;

    [SerializeField] protected string weaponProjectileCollisionSoundName;

    /*[Header("Bullet Trail")]
    [SerializeField] private LineRenderer trailRenderer;
    [SerializeField] private float trailDuration = 0.19f;
    [SerializeField] private float trailWidth = 0.1f;
    [SerializeField] private float trailLength = 50.0f;

    private Coroutine trailCoroutine;
    private bool trailCoroutineIsRunning = false;*/


    protected override void Start()
    {
        base.Start();
    }

    private void Awake()
    {
        //Effects
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }

        /*if (trail != null)
        {
            trail.gameObject.SetActive(false);
        }*/
    }


    public override void Attack(Vector3 targetPos)
    { 
        ShootProjectile(GetDirectionToTarget(targetPos));


        MuzzleFlashEffect();

        //BulletTrailEffect(targetPos);


        base.Attack(targetPos);
    }

    private void ShootProjectile(Vector3 direction)
    {
        GameObject projectileGO = Instantiate(projectilePrefab, HandGameObject.transform.position, Quaternion.LookRotation(-direction));

        Projectile projectile = projectileGO.GetComponent<Projectile>();

        projectile.Init(projectileSpeed, CollisionLayer, ProjectileCollision, projectileDetonateAfter, true, true);
    }


    private void ProjectileCollision(Vector3 position, GameObject col)
    {
        GameController.Instance.AudioController.Play(weaponProjectileCollisionSoundName);


        List<EnemyStats> enemies = GetEnemies(position);

        ApplyDamage(enemies);
    }


    protected override List<EnemyStats> GetEnemies(Vector3 targetPos)
    {
        List<EnemyStats> enemies = new();

        RaycastHit[] hits = ExtendedPhysics.OrderedSphereCastAll(targetPos, projectileDetonateRange / 2, Vector3.forward, projectileDetonateRange, CollisionLayer);

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
                float damage = (Damage - (Damage / 4)) + ((Damage / 4) * enemyFract);

                enemy.TakeDamage(damage);

                damagedEnemies++;
            }
        }
    }


    //Effects
    private void MuzzleFlashEffect()
    {
        if (muzzleFlash != null)
        {
            StartCoroutine(MuzzleFlash(muzzleFlashDuration));
        }
    }
    private IEnumerator MuzzleFlash(float muzzleFlashDuration)
    {
        muzzleFlash.SetActive(true);

        yield return new WaitForSeconds(muzzleFlashDuration);

        muzzleFlash.SetActive(false);
    }

    /*private void BulletTrailEffect(Vector3 targetPos)
    {
        if (trail != null && weaponBarrelEnd != null && playerController != null)
        {
            if (trailCoroutineIsRunning)
            {
                StopCoroutine(trailCoroutine);
                trailCoroutineIsRunning = false;
            }

            trail.widthMultiplier = trailWidth;

            trail.SetPosition(0, weaponBarrelEnd.position);

            float currentTrailLength = trailLength;
            if (collisionPosition != Vector3.zero)
            {
                currentTrailLength = Vector3.Distance(trail.GetPosition(0), collisionPosition);
            }

            //Direction on same Y level
            Vector3 risedPlayerPos = new Vector3(playerController.transform.position.x, weaponBarrelEnd.position.y, playerController.transform.position.z);
            Vector3 dir = GetDirectionToTarget(targetPos);

            trail.SetPosition(trail.positionCount - 1, risedPlayerPos + dir * currentTrailLength);

            trailCoroutine = StartCoroutine(BulletTrail(trailDuration));
        }
    }
    private IEnumerator BulletTrail(float trailDuration)
    {
        trailCoroutineIsRunning = true;

        float t = 0;
        float time = 0;

        trail.gameObject.SetActive(true);

        while (t < 1)
        {
            t = time / trailDuration;
            trail.widthMultiplier = Mathf.Lerp(trailWidth, 0, t);
            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        trail.widthMultiplier = trailWidth;

        trail.gameObject.SetActive(false);

        trailCoroutineIsRunning = false;
    }*/


    /*protected override void OnGUI()
    {
        base.OnGUI();

        if (GameController.Instance.ShowDebug && IsInUse)
        {
            GUI.color = Color.black;

            GUILayout.BeginArea(new Rect(Screen.width - 250f, (Screen.height / 2) + 50f, 250f, 25f));

            //GUILayout.Label($"penetrationAmount: {penetrationAmount}");

            GUILayout.EndArea();
        }
    }*/
}
    