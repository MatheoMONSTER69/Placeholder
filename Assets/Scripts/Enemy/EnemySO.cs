using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy", order = 1)]
public class EnemySO : ScriptableObject
{
    public string Name = "";
    public GameObject Prefab = null;

    public EnemyType EnemyType = EnemyType.Casual;

    [Header("Stats")]
    public float Health = 10.0f;
    public float Damage = 5.0f;
    public float MovingSpeed = 5.0f;

    public int PointsForKill = 1;

    [Header("Combat")]
    public float AttackRange = 1.0f;
    public float AttackCooldown = 1.0f;
}