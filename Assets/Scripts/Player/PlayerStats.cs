using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerStats : Stats
{
    [Header("Health Rregeneration")]
    [SerializeField] private float healthRegen = 5.0f;
    [SerializeField] private float regenTime = 3.0f;

    private Cooldown regenCooldown;


    private void Start()
    {
        regenCooldown = new Cooldown(regenTime);
    }

    private void Update()
    {
        if(Health != MaxHealth && !regenCooldown.IsInCooldown)
        {
            Heal(healthRegen);
            regenCooldown.StartCooldown();
        }
    }


    public override void TakeDamage(float amount)
    {
        if(!GameController.Instance.PlayerController.IsDodging)
        {
            base.TakeDamage(amount);
            regenCooldown.StartCooldown();
        }
    }

    private void OnGUI()
    {
        if (GameController.Instance.ShowDebug)
        {
            GUI.color = Color.black;

            GUILayout.BeginArea(new Rect(0, (Screen.height / 2) + 50f, 200f, 50f));

            GUILayout.Label($"Player Health: {Health} / {MaxHealth}");

            GUILayout.EndArea();
        }
    }
}
