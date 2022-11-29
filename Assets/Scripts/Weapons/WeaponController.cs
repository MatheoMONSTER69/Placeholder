using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class WeaponController : MonoBehaviour
{
    [HideInInspector] public Weapon CurrentWeapon => Weapons.Count > 0 ? Weapons[CurrentWeaponId] : null;
    [HideInInspector] public Weapon PrevWeapon => Weapons.Count > 0 && PrevWeaponId != -1 ? Weapons[PrevWeaponId] : null;
	[HideInInspector] public int PrevWeaponId = -1;
    [HideInInspector] public int CurrentWeaponId = 0;

    [Header("References")]
    public List<Weapon> Weapons = new();
    public Weapon Meele = null;

    [SerializeField] private Animator anim;

    [Header("Inputs")]
    private InputAction weaponChangeAxis;
    private InputAction weaponChangePrevButton;
    private InputAction weaponChangeNextButton;
    private InputAction prevWeapon;
    private InputAction meeleInput;

    private Cooldown switchCooldown = new(1);


    private void Awake()
    {
        weaponChangeAxis = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.WeaponChangeAxis);
        weaponChangePrevButton = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.WeaponChangePrevButton);
        weaponChangeNextButton = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.WeaponChangeNextButton);
        prevWeapon = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.PrevWeapon);
        meeleInput = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.Meele);

        //TODO: Add 1,2,3,4... support

        foreach (Weapon weapon in Weapons)
        {
            weapon.ShowBack();
        }
        Meele.ShowBack();

        anim.SetInteger("Weapon", 0);
        anim.SetBool("WeaponSwap", false);
    }

    private void Update()
    {
        if((!switchCooldown.IsStarted || switchCooldown.CooldownEnded) && !Meele.IsInUse)
        {
            float weaponChangeValue = weaponChangeAxis.ReadValue<float>();

            if (weaponChangeValue > 0 || weaponChangeNextButton.triggered)
            {
                WeaponUp();
            }
            else if (weaponChangeValue < 0 || weaponChangePrevButton.triggered)
            {
                WeaponDown();
            }

            if (prevWeapon.triggered)
            {
                PreviousWeapon();
            }

            if (meeleInput.triggered)
            {
                anim.SetTrigger("Meele");
            }
        }
    }


    public void PreviousWeapon()
    {
        int tempId = CurrentWeaponId;
        CurrentWeaponId = PrevWeaponId;
        PrevWeaponId = tempId;

        LoadCurrentWeapon();
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

        LoadCurrentWeapon();
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

        LoadCurrentWeapon();
    }

    public void ShowMeele()
    {
        Meele.ShowHand();
    }
    public void HideMeele()
    {
        Meele.ShowBack();
    }

    public void LoadCurrentWeapon()
    {
        switchCooldown = new(CurrentWeapon.SwitchTime);
        switchCooldown.StartCooldown();

        anim.SetInteger("Weapon", (int)CurrentWeapon.WeaponAnimStance);
        anim.SetBool("WeaponSwap", true);
    }
    public void SwapVisibleWeapon()
    {
        PrevWeapon.ShowBack();
        CurrentWeapon.ShowHand();

        anim.SetBool("WeaponSwap", false);
    }


    private void OnGUI()
    {
        if (GameController.Instance.ShowDebug)
        {
            GUI.color = Color.black;

            GUILayout.BeginArea(new Rect(Screen.width - 250f, Screen.height - 75f, 250f, 75f));

            GUILayout.Label($"CurrentWeapon: {CurrentWeapon}");
            GUILayout.Label($"PrevWeapon: {PrevWeapon}");
            GUILayout.Label($"MeeleInUse: {Meele.IsInUse}");

            GUILayout.EndArea();
        }
    }
}
