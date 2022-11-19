using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private Transform playerModel;
	private WeaponController weaponController;
	[HideInInspector] public PlayerStats stats;


	private void Start()
	{
		if(GameController.Instance != null)
		{
			GameController.Instance.PlayerController = this;
		}
		
		stats = GetComponent<PlayerStats>();
		weaponController = GetComponent<WeaponController>();
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
		//TODO
	}

	private void CopyPositionToModel()
	{
		playerModel.transform.position = transform.position;
	}
	private void CopyRotationToModel()
	{
		//TODO
	}
}
