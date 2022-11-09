using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy", order = 1)]
public class EnemySO : ScriptableObject
{
    public string Name = "";
    public GameObject Prefab = null;

    [Header("Stats")]
    public float Health = 10.0f;
    public float Damage = 5.0f;
    public float MovingSpeed = 5.0f;

    public int PointsForKill = 1;
}