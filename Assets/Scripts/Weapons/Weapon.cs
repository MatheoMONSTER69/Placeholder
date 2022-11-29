using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public WeaponSO WeaponScriptableObject;

    public GameObject BackGameObject = null;
    public GameObject HandGameObject = null;
    public WeaponAnimStance WeaponAnimStance = WeaponAnimStance.Long;

    public bool IsInUse => HandGameObject.activeInHierarchy;

    [Header("Settings")]
    protected float Damage;
    protected float AttackSpeed = 0.1f;
    public float SwitchTime = 1.5f;

    protected Cooldown AttackCooldown;


    private void Start()
    {
        if(WeaponScriptableObject != null)
        {
            Damage = WeaponScriptableObject.Damage;
            AttackSpeed = WeaponScriptableObject.AttackSpeed;
            SwitchTime = WeaponScriptableObject.SwitchTime;
        }

        AttackCooldown = new Cooldown(AttackSpeed);

        ShowBack();
    }


    public virtual void Attack()
    {
        AttackCooldown.StartCooldown();
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
}
