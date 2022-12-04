using UnityEngine;

public class Cooldown
{
    public bool IsInCooldown => Time.time <= _nextFireTime;

    private readonly float _cooldownTime;
    private float _nextFireTime = 0;


    public Cooldown(float cooldownTime, bool startWhenInit = false)
    {
        _cooldownTime = cooldownTime;

        if (startWhenInit)
        {
            StartCooldown();
        } 
    }

    public void StartCooldown()
    {
        _nextFireTime = Time.time + _cooldownTime;
    }
}