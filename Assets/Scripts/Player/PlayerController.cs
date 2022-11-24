using System.ComponentModel;
using System.Security.Cryptography;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
	public GameObject shrek_aim_ball;

	[Header("Settings")]
	[SerializeField] private float movementSpeed = 10.0f;
	[SerializeField] private float rotationSpeed = 5.0f;

	[Header("Inputs")]
	private InputAction pointerPosition;
	private InputAction movement;
	private InputAction dodge;
	private InputAction attack;
	public float AnimationTransitionSpeed = 0.2f;

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
		anim = playerModel.GetChild(0).GetComponent<Animator>();

		mainCam = Camera.main;
	}

	private void Update()
	{
		GetPlayerInput();
	}

	private void FixedUpdate()
	{
		CopyPositionToModel();
		CopyRotationToModel();
	}


	private void GetPlayerInput()
	{
		Vector2 playerMovement = movement.ReadValue<Vector2>();
		MovePlayer(playerMovement);

		Vector2 pointerScreenPosVal = GetPointerValue();
		RotatePlayer(PointerToWorldPos(pointerScreenPosVal));
        shrek_aim_ball.transform.position = PointerToWorldPos(pointerScreenPosVal);


		


        anim.SetFloat("WalkY", playerMovement.y, AnimationTransitionSpeed, Time.deltaTime);
        anim.SetFloat("WalkX", playerMovement.x, AnimationTransitionSpeed, Time.deltaTime);


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
		Vector2 targetPointerPos = new Vector2(Screen.width / 2, Screen.height / 2); //middle of the screen

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
		Vector3 newInputPos = new Vector3(input.x, 0, input.y);
		
		//TODO: Change to Rigidbody or CharacterController
		transform.position += newInputPos * movementSpeed * Time.deltaTime;
	}

	private void RotatePlayer(Vector3 lookAtPoint)
    {
        //TODO: Change to Leprp

		transform.LookAt(new Vector3(lookAtPoint.x, transform.position.y, lookAtPoint.z));
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
		anim.SetTrigger("Dodge");

		//TODO: Add
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
