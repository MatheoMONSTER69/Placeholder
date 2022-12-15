using UnityEngine;

/// <summary>
/// Use on object with animator to launch animation events
/// </summary>
public class WeaponSwitch : MonoBehaviour
{
    [SerializeField] private WeaponController weaponController;


    public void SwapVisibleWeapon()
    {
        weaponController.SwapVisibleWeapon();
    }

    public void HideMeele()
    {
        weaponController.HideMeele();
    }
    public void ShowMeele()
    {
        weaponController.ShowMeele();
    }
}
