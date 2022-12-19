using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public GameObject BackGameObject = null;
    public GameObject HandGameObject = null;
    public WeaponAnimStance WeaponAnimStance = WeaponAnimStance.Long;
    [SerializeField] protected Transform weaponBarrelEnd;
    [SerializeField] protected LayerMask CollisionLayer;
    protected PlayerController playerController;

    [Header("Settings")]
    public string Name = "Weapon";
    public float Damage = 5;
    public float AttackSpeed = 0.1f;
    public float SwitchTime = 1.5f; //1.5 is anim time

    [SerializeField] protected string weaponSoundName;

    public Cooldown AttackCooldown;

    public bool IsInUse => HandGameObject.activeInHierarchy;


    protected virtual void Start()
    {
        AttackCooldown = new Cooldown(AttackSpeed);

        ShowBack();

        if(GameController.Instance != null)
        {
            playerController = GameController.Instance.PlayerController;
        }
    }


    public virtual void Attack(Vector3 targetPos)
    {
        GameController.Instance.AudioController.Play(weaponSoundName);

        AttackCooldown.StartCooldown();
    }

    //Meant to be overridden
    protected virtual List<EnemyStats> GetEnemies(Vector3 targetPos) { return new List<EnemyStats>(); }

    protected virtual Vector3 GetDirectionToTarget(Vector3 targetPos)
    {
        Vector3 risedPlayerPos = new Vector3(playerController.transform.position.x, weaponBarrelEnd.position.y, playerController.transform.position.z);
        Vector3 raisedTargetPos = new Vector3(targetPos.x, weaponBarrelEnd.position.y, targetPos.z);

        return (raisedTargetPos - risedPlayerPos).normalized;
    }
    protected virtual RaycastHit[] RaycastEnemies(Vector3 targetPos)
    {
        Vector3 risedPlayerPos = new Vector3(playerController.transform.position.x, weaponBarrelEnd.position.y, playerController.transform.position.z);

        Vector3 dir = GetDirectionToTarget(targetPos);

        return ExtendedPhysics.OrderedRaycastAll(risedPlayerPos, dir, float.MaxValue, CollisionLayer);
    }

    protected virtual void ApplyDamage(List<EnemyStats> enemies)
    {
        if (enemies.Count > 0)
        {
            foreach (EnemyStats enemy in enemies)
            {
                enemy.TakeDamage(Damage);
            }
        }
    }


    public void ShowBack()
    {
        BackGameObject.SetActive(true);
        HandGameObject.SetActive(false);
    }
    public void ShowHand()
    {
        HandGameObject.SetActive(true);
        BackGameObject.SetActive(false);
    }


    protected virtual void OnGUI()
    {
        if (GameController.Instance.ShowDebug && IsInUse)
        {
            GUI.color = Color.black;

            GUILayout.BeginArea(new Rect(Screen.width - 250f, (Screen.height / 2) - 75f, 250f, 125f));

            GUILayout.Label($"Name: {Name}");
            GUILayout.Label($"Damage: {Damage}");
            GUILayout.Label($"AttackSpeed: {AttackSpeed}");
            GUILayout.Label($"SwitchTime: {SwitchTime}");
            GUILayout.Label($"AttackCooldown: {AttackCooldown.IsInCooldown}");

            GUILayout.EndArea();
        }
    }
}
