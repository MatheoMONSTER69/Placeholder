using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stats : MonoBehaviour
{
    [Header("Stats")]
    public float MaxHealth;
    public float Health { get; private set; }
    public float Damage;

    public bool IsDead = false;

    [Header("Events")]
    public UnityEvent OnDamageTaken;
    public UnityEvent OnHeal;
    public UnityEvent OnDeath;


    public virtual void TakeDamage(float amount)
    {
        Health = Mathf.Clamp(Health - amount, 0, MaxHealth);

        OnDamageTaken.Invoke();

        if(Health <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        if(!IsDead)
        {
            Health = Mathf.Clamp(Health + amount, 0, MaxHealth);

            OnHeal.Invoke();
        }
    }

    public virtual void Die()
    {
        IsDead = true;

        OnDeath.Invoke();
    }
}
