using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]

    public GameObject BackGameObject = null;
    public GameObject HandGameObject = null;
    public WeaponAnimStance WeaponAnimStance = WeaponAnimStance.Long;

    [Header("Settings")]
    public string Name = "Weapon";
    public float Damage = 5;
    public float AttackSpeed = 0.1f;
    public float SwitchTime = 1.5f;

    protected Cooldown AttackCooldown;

    public bool IsInUse => HandGameObject.activeInHierarchy;


    private void Start()
    {
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
