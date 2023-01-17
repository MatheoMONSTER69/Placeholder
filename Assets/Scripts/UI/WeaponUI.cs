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
        Invoke(nameof(InvokedEnable), 1);
    }

    private void Update()
    {
        //SetWeapon(weaponSprites);
    }
    private void InvokedEnable()
    {
        GameController.Instance.WeaponController.OnWeaponSwitch.AddListener(() => SetWeapon(weaponSprites));
    }
    private void OnDisable()
    {
        GameController.Instance.WeaponController.OnWeaponSwitch.AddListener(() => SetWeapon(weaponSprites));
    }

    private void SetWeapon(Sprite[] sprites)
    {
        spriteRenderer.sprite = sprites[GameController.Instance.WeaponController.CurrentWeaponId];
    }
}
