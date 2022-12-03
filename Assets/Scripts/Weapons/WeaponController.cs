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
    private InputAction prevWeapon;
    private InputAction meeleInput;

    private Cooldown switchCooldown = new(1);


    private void Awake()
    {
        weaponChangeAxis = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.WeaponChangeAxis);
        prevWeapon = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.PrevWeapon);
        meeleInput = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.Meele);

        foreach (Weapon weapon in Weapons)
        {
            weapon.ShowBack();
        }
        Meele.ShowBack();

        EquipWeapon(0);
    }

    private void Update()
    {
        if((!switchCooldown.IsStarted || switchCooldown.CooldownEnded) && !Meele.IsInUse)
        {
            float weaponChangeValue = weaponChangeAxis.ReadValue<float>();

            if (weaponChangeValue > 0)
            {
                //1-9 keys
                if (int.TryParse(weaponChangeAxis.activeControl.name, out int key))
                {
                    EquipWeapon(key - 1);
                }
                //scroll / mouse buttons
                else
                {
                    EquipWeaponUp();
                }
            }
            //scroll / mouse buttons
            else if (weaponChangeValue < 0)
            {
                EquipWeaponDown();
            }

            if (prevWeapon.triggered)
            {
                EquipPreviousWeapon();
            }

            if (meeleInput.triggered)
            {
                MeeleAttack(transform.position);
            }
        }
    }


    public void EquipPreviousWeapon()
    {
        EquipWeapon(PrevWeaponId);
    }

    public void EquipWeaponDown()
    {
        if (CurrentWeaponId == 0)
        {
            EquipWeapon(Weapons.Count - 1);
        }
        else
        {
            EquipWeapon(CurrentWeaponId - 1);
        }
    }
    public void EquipWeaponUp()
    {
        if(CurrentWeaponId == Weapons.Count - 1)
        {
            EquipWeapon(0);
        }
        else
        {
            EquipWeapon(CurrentWeaponId + 1);
        }
    }

    public void EquipWeapon(int weaponId)
    {
        if(Weapons.Count > 0 && weaponId < Weapons.Count)
        {
            PrevWeaponId = CurrentWeaponId;

            CurrentWeaponId = weaponId;

            LoadCurrentWeapon();
        }
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


    public void Attack(Vector3 targetPos)
    {
        if(
            CurrentWeapon != null && 
            (CurrentWeapon != Meele && !Meele.IsInUse) &&
            (!switchCooldown.IsStarted || switchCooldown.CooldownEnded) &&
            (!CurrentWeapon.AttackCooldown.IsStarted || CurrentWeapon.AttackCooldown.CooldownEnded))
        {
            CurrentWeapon.Attack(targetPos);
        }
    }
    public void MeeleAttack(Vector3 targetPos)
    {
        if (
            CurrentWeapon != null &&
            (CurrentWeapon != Meele && !Meele.IsInUse) &&
            (!switchCooldown.IsStarted || switchCooldown.CooldownEnded) &&
            (!CurrentWeapon.AttackCooldown.IsStarted || CurrentWeapon.AttackCooldown.CooldownEnded) &&
            (!Meele.AttackCooldown.IsStarted || Meele.AttackCooldown.CooldownEnded))
        {
            anim.SetTrigger("Meele");
            Meele.Attack(targetPos);
        }
    }


    private void OnGUI()
    {
        if (GameController.Instance.ShowDebug)
        {
            GUI.color = Color.black;

            GUILayout.BeginArea(new Rect(Screen.width - 250f, Screen.height - 100f, 250f, 100f));

            GUILayout.Label($"CurrentWeapon: {CurrentWeapon}");
            GUILayout.Label($"PrevWeapon: {PrevWeapon}");
            GUILayout.Label($"MeeleInUse: {Meele.IsInUse}");
            GUILayout.Label($"SwitchCooldown: {switchCooldown.IsStarted}");

            GUILayout.EndArea();
        }
    }
}
