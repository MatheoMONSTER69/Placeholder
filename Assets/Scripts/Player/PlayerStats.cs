using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    public override void Die()
    {
        GameController.Instance.StopGame();

        base.Die();
    }
}
