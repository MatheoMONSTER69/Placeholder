using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    WeaponController weaponController;

    public SpriteRenderer spriteRenderer;
    public Sprite[] weaponSprites;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        weaponController = GameController.Instance.WeaponController;
    }
    private void OnEnable()
    {
        Invoke(nameof(InvokedEnable), 0.1f);
    }

    private void InvokedEnable()
    {
        GameController.Instance.WeaponController.OnWeaponSwitch.AddListener(SetWeapon);
    }
    private void OnDisable()
    {
        GameController.Instance.WeaponController.OnWeaponSwitch.RemoveListener(SetWeapon);
    }

    private void SetWeapon()
    {
        spriteRenderer.sprite = weaponSprites[GameController.Instance.WeaponController.CurrentWeaponId];
    }
}
