using System.ComponentModel;
using System.Security.Cryptography;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static Unity.Burst.Intrinsics.X86;

public class PlayerController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Transform playerModel;
	private Camera mainCam;
	private WeaponController weaponController;
	[HideInInspector] public PlayerStats stats;
	private Animator anim;
    private CharacterController controller;

    [Header("Settings")]
	[SerializeField] private float movementSpeed = 5.0f;
	[SerializeField] private float rotationSpeed = 15.0f;
    [SerializeField] private float AnimationTransitionSpeed = 0.2f;

    [Header("Inputs")]
	private InputAction pointerPosition;
	private InputAction movement;
	private InputAction dodge;
	private InputAction attack;

	[SerializeField] private LayerMask inputPlaneLayer;

	[Header("Debug")]
	[SerializeField] private bool showDebug = false;


	private void Awake()
	{
		pointerPosition = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.PointerPosition);
		movement = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.Movement);
		dodge = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.Dodge);
		attack = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.Attack);
	}

	private void Start()
	{
		if(GameController.Instance != null)
		{
			GameController.Instance.PlayerController = this;
		}
		
		stats = GetComponent<PlayerStats>();
		weaponController = GetComponent<WeaponController>();
		anim = playerModel.GetComponent<Animator>();
        controller = gameObject.GetComponent<CharacterController>();

        mainCam = Camera.main;
	}

	private void Update()
	{
		GetPlayerInput();
	}

	private void LateUpdate()
	{
		CopyPositionToModel();
		CopyRotationToModel();
	}


	private void GetPlayerInput()
	{
		Vector2 playerMovement = movement.ReadValue<Vector2>();
		MovePlayer(playerMovement);

        Vector2 pointerScreenPosVal = GetPointerValue();
		if(pointerScreenPosVal != Vector2.zero)
		{
            RotatePlayer(PointerToWorldPos(pointerScreenPosVal));
        }
		

        if (attack.triggered)
        {
			Attack();
        }

		if(dodge.triggered)
        {
			Dodge();
        }
	}

	private Vector3 GetPointerValue()
	{
		Vector2 targetPointerPos = Vector2.zero;

		Vector2 pointerPos = pointerPosition.ReadValue<Vector2>();

        if (/*!EventSystem.current.IsPointerOverGameObject() &&*/
			pointerPos.x <= Screen.width && pointerPos.x >= 0 &&
			pointerPos.y <= Screen.height && pointerPos.y >= 0)
		{
			targetPointerPos = pointerPosition.ReadValue<Vector2>();
		}

		return targetPointerPos;
	}
	private Vector3 PointerToWorldPos(Vector2 pointerScreenPos)
	{
		Ray ray = mainCam.ScreenPointToRay(pointerScreenPos);

		if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, inputPlaneLayer))
		{
			return raycastHit.point;
		}

		return Vector3.zero;
	}

	private void MovePlayer(Vector2 input)
    {
        Vector3 right = mainCam.transform.right;
        Vector3 forward = mainCam.transform.forward;

        right.y = 0f;
        forward.y = 0f;

        Vector3 moveDirection = right.normalized * input.x + forward.normalized * input.y;

		controller.Move(moveDirection * movementSpeed * Time.deltaTime);


		//TODO: Improve
        Vector3 facing = new Vector3(transform.forward.x * input.x, 0, transform.forward.z * input.y);

        anim.SetFloat("WalkX", -facing.x, AnimationTransitionSpeed, Time.deltaTime);
        anim.SetFloat("WalkY", -facing.z, AnimationTransitionSpeed, Time.deltaTime);
    }

	private void RotatePlayer(Vector3 lookAtPoint)
    {
        Vector3 targetDirection = lookAtPoint - transform.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);

        transform.rotation = Quaternion.LookRotation(new Vector3(newDirection.x, 0, newDirection.z));
    }


	private void CopyPositionToModel()
	{
		playerModel.transform.position = transform.position;
	}
	private void CopyRotationToModel()
	{
		playerModel.transform.rotation = transform.rotation;
	}

	private void Attack()
    {
		if(weaponController.CurrentWeapon != null)
		{
			weaponController.CurrentWeapon.Attack();
		}
    }

	private void Dodge()
    {
		anim.SetTrigger("Roll");

		//TODO: Implement
    }


	private void OnDrawGizmos()
	{
		if(showDebug)
		{
			if (GameController.Instance != null && GameController.Instance.IsGameStarted)
			{
				Gizmos.color = Color.red;
				Vector2 pointerScreenPosVal = GetPointerValue();
				Gizmos.DrawWireSphere(PointerToWorldPos(pointerScreenPosVal), 0.25f);
			}
		}
	}
}
