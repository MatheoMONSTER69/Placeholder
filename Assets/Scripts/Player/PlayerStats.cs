using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerStats : Stats
{
    public override void Die()
    {
        GameController.Instance.StopGame(false);

        base.Die();
    }

    public override void TakeDamage(float amount)
    {
        if(!GameController.Instance.PlayerController.IsDodging)
        {
            base.TakeDamage(amount);
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
