using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponSO WeaponScriptableObject;

    protected float Damage;
    protected float AttackSpeed;
    protected float SwitchTime;

    protected Cooldown SwitchColldown;
    protected Cooldown AttackCooldown;


    private void Start()
    {
        Damage = WeaponScriptableObject.Damage;
        AttackSpeed = WeaponScriptableObject.AttackSpeed;
        SwitchTime = WeaponScriptableObject.SwitchTime;

        AttackCooldown = new Cooldown(AttackSpeed);
        SwitchColldown = new Cooldown(SwitchTime);
    }

    private void Awake()
    {
        SwitchColldown.StartCooldown();
    }


    public void Attack()
    {
        AttackCooldown.StartCooldown();
    }
}
