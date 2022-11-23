using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class WeaponController : MonoBehaviour
{
    [HideInInspector] public Weapon CurrentWeapon => Weapons.Count > 0 ? Weapons[CurrentWeaponId] : null;
    [HideInInspector] public Weapon PrevWeapon => Weapons.Count > 0 ? Weapons[PrevWeaponId] : null;
	[HideInInspector] public int PrevWeaponId = 0;
    [HideInInspector] public int CurrentWeaponId = 0;

    public List<Weapon> Weapons = new();

    private InputAction weaponChange;
    private InputAction prevWeapon;


    private void Awake()
    {
		weaponChange = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.WeaponChange);
		prevWeapon = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.PrevWeapon);
    }

    private void Update()
    {
		float weaponChangeValue = weaponChange.ReadValue<float>();
        if(weaponChangeValue > 0)
        {
			WeaponUp();
		}
        else if(weaponChangeValue < 0)
        {
			WeaponDown();
		}

        if(prevWeapon.triggered)
        {
			PreviousWeapon();
        }
    }


    public void PreviousWeapon()
    {
        int tempId = CurrentWeaponId;
        CurrentWeaponId = PrevWeaponId;
        PrevWeaponId = tempId;
    }

    public void WeaponDown()
    {
        PrevWeaponId = CurrentWeaponId;

        if(CurrentWeaponId == 0)
        {
            CurrentWeaponId = Weapons.Count - 1;
        }
        else
        {
            CurrentWeaponId--;
        }
    }

    public void WeaponUp()
    {
        PrevWeaponId = CurrentWeaponId;

        if(CurrentWeaponId == Weapons.Count - 1)
        {
            CurrentWeaponId = 0;
        }
        else
        {
            CurrentWeaponId++;
        }
    }
}
