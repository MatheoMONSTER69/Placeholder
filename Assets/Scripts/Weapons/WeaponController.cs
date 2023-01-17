using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour
{
    [HideInInspector] public Weapon CurrentWeapon => enabledWeapons.Count > CurrentWeaponId ? enabledWeapons[CurrentWeaponId] : null;
    [HideInInspector] public Weapon PrevWeapon => enabledWeapons.Count > PrevWeaponId && PrevWeaponId != -1 ? enabledWeapons[PrevWeaponId] : null;
	[HideInInspector] public int PrevWeaponId = -1;
    [HideInInspector] public int CurrentWeaponId = 0;

    [Header("References")]
    public List<Weapon> Weapons = new();
    private List<Weapon> enabledWeapons;
    public Weapon Meele = null;

    [SerializeField] private Animator anim;

    [Header("Inputs")]
    private InputAction weaponChangeAxis;
    private InputAction prevWeapon;
    private InputAction meeleInput;

    private Cooldown switchCooldown = new(1);

    [Header("Events")]
    public UnityEvent OnWeaponSwitch;


    private void Start()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.WeaponController = this;
        }

        weaponChangeAxis = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.WeaponChangeAxis);
        prevWeapon = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.PrevWeapon);
        meeleInput = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.Meele);

        GetEnabledWeapons();

        foreach (Weapon weapon in enabledWeapons)
        {
            weapon.ShowBack();
        }
        Meele.ShowBack();
    }

    private void Update()
    {
        if((!switchCooldown.IsInCooldown) && !Meele.IsInUse)
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
                    EquipWeaponUp(CurrentWeaponId);
                }
            }
            //scroll / mouse buttons
            else if (weaponChangeValue < 0)
            {
                EquipWeaponDown(CurrentWeaponId);
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

    public void EquipWeaponDown(int weaponId)
    {
        if (weaponId == 0)
        {
            EquipWeapon(enabledWeapons.Count - 1);
        }
        else
        {
            EquipWeapon(weaponId - 1);
        }
    }
    public void EquipWeaponUp(int weaponId)
    {
        if(weaponId == enabledWeapons.Count - 1)
        {
            EquipWeapon(0);
        }
        else
        {
            EquipWeapon(weaponId + 1);
        }
    }

    public void EquipWeapon(int weaponId)
    {
        if(enabledWeapons.Count > 0 && weaponId < enabledWeapons.Count && weaponId != CurrentWeaponId)
        {
            if (enabledWeapons[weaponId].WeaponEnabled)
            {
                PrevWeaponId = CurrentWeaponId;

                CurrentWeaponId = weaponId;

                LoadCurrentWeapon();
            }
            else
            {
                //Weapon is disabled, skip it
                if(CurrentWeaponId > weaponId)
                {
                    EquipWeaponDown(weaponId);
                }
                else if(CurrentWeaponId < weaponId)
                {
                    EquipWeaponUp(weaponId);
                }
            }
        }
        OnWeaponSwitch.Invoke();
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
        if(PrevWeapon != null)
        {
            PrevWeapon.ShowBack();
        }
        
        if(CurrentWeapon != null)
        {
            CurrentWeapon.ShowHand();
        }

        anim.SetBool("WeaponSwap", false);
    }

    [ContextMenu("GetEnabledWeapons")]
    public void GetEnabledWeapons()
    {
        enabledWeapons = Weapons.Where(x => x.WeaponEnabled == true).ToList();
    }


    public void Attack(Vector3 targetPos)
    {
        if(
            CurrentWeapon != null && 
            (CurrentWeapon != Meele && !Meele.IsInUse) &&
            !switchCooldown.IsInCooldown &&
            !CurrentWeapon.AttackCooldown.IsInCooldown)
        {
            CurrentWeapon.Attack(targetPos);
        }
    }
    public void MeeleAttack(Vector3 targetPos)
    {
        if (
            CurrentWeapon != null &&
            (CurrentWeapon != Meele && !Meele.IsInUse) &&
            !switchCooldown.IsInCooldown &&
            !CurrentWeapon.AttackCooldown.IsInCooldown &&
            !Meele.AttackCooldown.IsInCooldown)
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
            GUILayout.Label($"SwitchCooldown: {switchCooldown.IsInCooldown}");

            GUILayout.EndArea();
        }
    }
}
