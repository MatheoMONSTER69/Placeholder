using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private Transform playerModel;
    [SerializeField] public int weaponNr;
    [SerializeField] private GameObject[] BackWeapons;
    [SerializeField] private GameObject[] HandWeapons;
    [SerializeField] private GameObject Melee;
    int currentWeapon;

    void Start()
    {
        anim = this.GetComponent<Animator>();


        anim.SetInteger("Weapon", 0);
     
        DisableEnableWeapons(HandWeapons, false);
        anim.SetBool("WeaponSwap", false);
        DisableEnableWeapons(BackWeapons, true);
        
    }


    /// TO JEST PLACEHOLDER DO TESTOWANIA ZMIANY BRONI, ODBIERDOLTA SIE ODEMNIE, MOJEGO KODU I ODEMNIE 

    void Update()
    {
        
        
        if (weaponNr == 0 && weaponNr!=currentWeapon) // uzi
        {
            anim.SetInteger("Weapon", 0);
            anim.SetBool("WeaponSwap", true);
            
        }
        else if (weaponNr == 1 && weaponNr != currentWeapon) // ak
        {
            anim.SetInteger("Weapon", 1);
            anim.SetBool("WeaponSwap", true);
            
        }
        else if (weaponNr == 2 && weaponNr != currentWeapon) // shotgun
        {
            anim.SetInteger("Weapon", 1);
            anim.SetBool("WeaponSwap", true);
           
        }
        else if (weaponNr == 3 && weaponNr != currentWeapon) // rpg
        {
            anim.SetInteger("Weapon", 1);
            anim.SetBool("WeaponSwap", true);
            
        }
       
        currentWeapon = weaponNr;
    }
    public void SwapVisibleWeapon()
    {
        DisableEnableWeapons(BackWeapons, true);
        BackWeapons[weaponNr].active = false;
        DisableEnableWeapons(HandWeapons, false);
        HandWeapons[weaponNr].active = true;


        anim.SetBool("WeaponSwap", false);
    }
    public void meleINVisible()
    {
        Melee.active = false;
    }
    public void meleVisible()
    {
        Melee.active = true;
    }

    private void DisableEnableWeapons(GameObject[] array, bool state)
    {
        foreach(GameObject x in array)
        {
            x.active = state;
        }
    }
  
}
