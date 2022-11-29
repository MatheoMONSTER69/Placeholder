using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public SerializableDictionary<EnemySO, int> Enemies;
    public float Cooldown = 10;

    [Header("Stats")]
    [HideInInspector] public bool Finished = false;
    [HideInInspector] public double Time = 0;
    [HideInInspector] public int WaveScore = 0;
}
