using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rifle : Weapon
{
    [Header("Rifle")]
    [SerializeField] private bool usePenetration = false;

    [Header("Effects")]
    [Header("Muzzle Flash")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private float muzzleFlashDuration = 0.05f;

    [Header("Bullet Trail")]
    [SerializeField] private LineRenderer trail;
    [SerializeField] private float trailDuration = 0.19f;
    [SerializeField] private float trailWidth = 0.1f;
    [SerializeField] private float trailLength = 50.0f;

    private Coroutine trailCoroutine;
    private bool trailCoroutineIsRunning = false;


    protected override void Start()
    {
        base.Start();
    }

    private void Awake()
    {
        //Effects
        if(muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }

        if(trail != null)
        {
            trail.gameObject.SetActive(false);
        }
    }


    public override void Attack(Vector3 targetPos)
    {
        List<EnemyStats> enemies = GetEnemies(targetPos);

        ApplyDamage(enemies);


        MuzzleFlashEffect();

        BulletTrailEffect(targetPos, enemies.Count > 0 ? enemies[0].transform : null);


        base.Attack(targetPos);
    }


    protected override List<EnemyStats> GetEnemies(Vector3 targetPos)
    {
        List<EnemyStats> enemies = new();

        if(weaponBarrelEnd != null)
        {
            Vector3 dir = (new Vector3(targetPos.x, weaponBarrelEnd.position.y, targetPos.z) - weaponBarrelEnd.position).normalized;

            RaycastHit[] hits = Physics.RaycastAll(weaponBarrelEnd.position, dir, float.MaxValue, enemyLayer);

            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.TryGetComponent(out EnemyStats enemyStats))
                    {
                        enemies.Add(enemyStats);

                        if (!usePenetration)
                        {
                            break;
                        }
                    }
                }
            }
        }

        return enemies;
    }

    protected override void ApplyDamage(List<EnemyStats> enemies)
    {
        if(enemies.Count > 0)
        {
            if (usePenetration)
            {
                foreach (EnemyStats enemy in enemies)
                {
                    float damage = Damage;

                    //TODO scale damage based on distance and amount of enemies hit (enemies.Count and Vector3.Distance(weaponBarrelEnd.position, enemy.transform.position))

                    enemy.TakeDamage(damage);
                }
            }
            else
            {
                enemies[0].TakeDamage(Damage);
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

    private void BulletTrailEffect(Vector3 targetPos, Transform enemy)
    {
        if (trail != null && weaponBarrelEnd != null)
        {
            if (trailCoroutineIsRunning)
            {
                StopCoroutine(trailCoroutine);
                trailCoroutineIsRunning = false;
            }

            trail.widthMultiplier = trailWidth;

            trail.SetPosition(0, weaponBarrelEnd.position);

            float currentTrailLength = trailLength;
            if (!usePenetration && enemy != null)
            {
                currentTrailLength = Vector3.Distance(trail.GetPosition(0), enemy.transform.position);
            }

            //Direction on same Y level
            Vector3 dir = (new Vector3(targetPos.x, weaponBarrelEnd.position.y, targetPos.z) - weaponBarrelEnd.position).normalized;
            trail.SetPosition(trail.positionCount - 1, weaponBarrelEnd.position + dir * currentTrailLength);

            trailCoroutine = StartCoroutine(BulletTrail(trailDuration));
        }
    }
    private IEnumerator BulletTrail(float trailDuration)
    {
        trailCoroutineIsRunning = true;

        float t = 0;
        float time = 0;

        trail.gameObject.SetActive(true);

        while (t<1)
        {
            t = time / trailDuration;
            trail.widthMultiplier = Mathf.Lerp(trailWidth, 0, t);
            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        trail.widthMultiplier = trailWidth;

        trail.gameObject.SetActive(false);

        trailCoroutineIsRunning = false;
    }


    protected override void OnGUI()
    {
        base.OnGUI();

        if (GameController.Instance.ShowDebug && IsInUse)
        {
            GUI.color = Color.black;

            GUILayout.BeginArea(new Rect(Screen.width - 250f, (Screen.height / 2) + 50f, 250f, 25f));

            GUILayout.Label($"usePenetration: {usePenetration}");

            GUILayout.EndArea();
        }
    }
}
