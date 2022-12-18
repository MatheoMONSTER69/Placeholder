using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class PhysicsBased : Weapon
{
    [Header("PhysicsBasedWeapon")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 1.0f;
    [SerializeField] private float detonateAfter = 5.0f;
    [SerializeField] private float detonateRange = 3.0f;

    private Vector3 bulletDir;
    private GameObject bulletGameObject;
    private Cooldown bulletDetonationCooldown;

    [Header("Effects")]
    [Header("Muzzle Flash")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private float muzzleFlashDuration = 0.05f;

    /*[Header("Bullet Trail")]
    [SerializeField] private LineRenderer trail;
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

        bulletDetonationCooldown = new(detonateAfter);
    }

    private void Update()
    {
        if(bulletGameObject != null)
        {
            bulletGameObject.transform.Translate(bulletDir * bulletSpeed);

            if(Physics.Raycast(bulletGameObject.transform.position, bulletGameObject.transform.forward, CollisionLayer) || !bulletDetonationCooldown.IsInCooldown)
            {
                BulletCollision(bulletGameObject.transform.position);
            }
        }
    }


    public override void Attack(Vector3 targetPos)
    {
        ShootBullet(GetDirectionToTarget(targetPos));


        MuzzleFlashEffect();

        //BulletTrailEffect(targetPos);


        base.Attack(targetPos);
    }

    private void ShootBullet(Vector3 direction)
    {
        bulletDetonationCooldown.StartCooldown();

        bulletGameObject = Instantiate(bulletPrefab);
        bulletDir = direction;
    }

    private void BulletCollision(Vector3 position)
    {
        Destroy(bulletGameObject);

        bulletGameObject = null;


        List<EnemyStats> enemies = GetEnemies(position);

        ApplyDamage(enemies);
    }



    protected override List<EnemyStats> GetEnemies(Vector3 targetPos)
    {
        List<EnemyStats> enemies = new();

        RaycastHit[] hits = ExtendedPhysics.OrderedSphereCastAll(targetPos, detonateRange / 2, Vector3.forward, detonateRange, CollisionLayer);

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

                enemy.TakeDamage(enemyFract);

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
    