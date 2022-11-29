using UnityEngine;

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
