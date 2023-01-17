using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider slider;

    private void OnEnable()
    {
        Invoke(nameof(InvokedEnable), 1);
    }
    private void InvokedEnable()
    {
        GameController.Instance.PlayerController.stats.OnDamageTaken.AddListener(SetHealth);
        GameController.Instance.PlayerController.stats.OnHeal.AddListener(SetHealth);
    }
    private void OnDisable()
    {
        GameController.Instance.PlayerController.stats.OnDamageTaken.RemoveListener(SetHealth);
        GameController.Instance.PlayerController.stats.OnHeal.AddListener(SetHealth);
    }

    public void SetHealth()
    {
        slider.value = MathUtils.RemapTo01(GameController.Instance.PlayerController.stats.Health, 0, GameController.Instance.PlayerController.stats.MaxHealth);
    }
}
