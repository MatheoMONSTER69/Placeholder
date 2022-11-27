using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private Transform playerModel;
    // Start is called before the first frame update
    [SerializeField] private int weapon;
  //  [SerializeField] private GameObject[] BackWeapons;
    [SerializeField] private GameObject[] HandWeapons;

    void Start()
    {
        anim = playerModel.GetComponent<Animator>();
       // weapon = 0;
    }


    /// TO JEST PLACEHOLDER DO TESTOWANIA ZMIANY BRONI, ODBIERDOLTA SIE ODEMNIE, MOJEGO KODU I ODEMNIE 

    void Update()
    {
        
        if (weapon == 1) // pistol
        {
            anim.SetInteger("Weapon", 2);
            SwapVisibleWeapon(1);
        }
        else if (weapon == 2) // rifle
        {
            anim.SetInteger("Weapon", 1);
            SwapVisibleWeapon(2);
        }
        else if (weapon == 3) // shotgun
        {
            anim.SetInteger("Weapon", 1);
            SwapVisibleWeapon(3);
        }
        else if (weapon == 4) // rpg
        {
            anim.SetInteger("Weapon", 1);
            SwapVisibleWeapon(4);
        }
        else if (weapon == 0) // empty
        {
            anim.SetInteger("Weapon", 0);
            SwapVisibleWeapon(0);
        }

    }
    private void SwapVisibleWeapon(int weapon)
    {
     //   DisableEnableWeapons(BackWeapons, true);
      //  BackWeapons[weapon].SetActive(false);
        DisableEnableWeapons(HandWeapons, false);
        HandWeapons[weapon].SetActive(true);


    }
    private void DisableEnableWeapons(GameObject[] array, bool state)
    {
        foreach(GameObject x in array)
        {
            x.SetActive(state);
        }
    }
  
}
