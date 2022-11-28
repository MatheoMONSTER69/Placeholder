using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private Transform playerModel;
    [SerializeField] public int weapon;
    [SerializeField] private GameObject[] BackWeapons;
    [SerializeField] private GameObject[] HandWeapons;
    private int currentWeapon;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        // weapon = 0;
        DisableEnableWeapons(HandWeapons, false);
        anim.SetInteger("Weapon", weapon);
        anim.SetBool("WeaponSwap", false);
        DisableEnableWeapons(BackWeapons, true);
    }


    /// TO JEST PLACEHOLDER DO TESTOWANIA ZMIANY BRONI, ODBIERDOLTA SIE ODEMNIE, MOJEGO KODU I ODEMNIE 

    void Update()
    {
        
        
        if (weapon == 0 && weapon!=currentWeapon) // uzi
        {
            anim.SetInteger("Weapon", 0);
            anim.SetBool("WeaponSwap", true);
            // SwapVisibleWeapon(1);
        }
        else if (weapon == 1 && weapon != currentWeapon) // ak
        {
            anim.SetInteger("Weapon", 1);
            anim.SetBool("WeaponSwap", true);
            // SwapVisibleWeapon(2);
        }
        else if (weapon == 2 && weapon != currentWeapon) // shotgun
        {
            anim.SetInteger("Weapon", 1);
            anim.SetBool("WeaponSwap", true);
            // SwapVisibleWeapon(3);
        }
        else if (weapon == 3 && weapon != currentWeapon) // rpg
        {
            anim.SetInteger("Weapon", 1);
            anim.SetBool("WeaponSwap", true);
            // SwapVisibleWeapon(4);
        }
       

        currentWeapon = weapon;
    }
    public void SwapVisibleWeapon()
    {
        DisableEnableWeapons(BackWeapons, true);
        BackWeapons[weapon].active = false;
        DisableEnableWeapons(HandWeapons, false);
        HandWeapons[weapon].active = true;
        anim.SetBool("WeaponSwap", false);


    }
    private void DisableEnableWeapons(GameObject[] array, bool state)
    {
        foreach(GameObject x in array)
        {
            x.active = state;
        }
    }
  
}
