using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
public class WeaponSO : ScriptableObject
{
    public string Name = "";
    public GameObject Prefab = null;

    [Header("Stats")]
    public float Damage = 5.0f;
    public float AttackSpeed = 1.0f;
}