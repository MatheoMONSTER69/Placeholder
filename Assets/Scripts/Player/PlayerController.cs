using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerModel;
    private Camera mainCam;
    private WeaponController weaponController;
    [HideInInspector] public PlayerStats stats;
    private Animator anim;
    private CharacterController controller;
    [SerializeField] private Transform aimTarget;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float gravity = 1.0f;

    [Space(10)]

    [SerializeField] private float AnimationTransitionSpeed = 0.2f;
    [SerializeField] private float dodgingTime = 1.0f;

    [Space(10)]

    [SerializeField] private bool rigidbodyForceEnabled = true;
    [SerializeField] private float rigidbodyPushForce = 2.0f;

    [Header("Inputs")]
    [SerializeField] private LayerMask inputPlaneLayer;

    private InputAction pointerPosition;
    private InputAction gamepadPosition;
    private InputAction movement;
    private InputAction dodge;
    private InputAction attack;

    private Vector2 movementInput = Vector2.zero;

    [Header("States")]
    [HideInInspector] public bool IsDodging = false;


    private void Awake()
    {
        pointerPosition = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.PointerPosition);
        gamepadPosition = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.GamepadPosition);
        movement = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.Movement);
        dodge = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.Dodge);
        attack = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.Attack); 
    }

    private void Start()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.PlayerController = this;
        }

        stats = GetComponent<PlayerStats>();
        weaponController = GetComponent<WeaponController>();
        anim = playerModel.GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        mainCam = Camera.main;

        stats.OnDamageTaken.AddListener(TakeDamage);
    }

    private void OnDisable()
    {
        stats.OnDamageTaken.RemoveListener(TakeDamage);
    }

    private void Update()
    {
        GetPlayerInput();

        MovePlayer(movementInput);
        RotatePlayer(aimTarget.position);
    }

    private void LateUpdate()
    {
        CopyPositionToModel();
        CopyRotationToModel();
    }


    private void GetPlayerInput()
    {
        if (!IsDodging)
        {
            movementInput = movement.ReadValue<Vector2>();
            movementInput = Vector3.ClampMagnitude(movementInput, 1); //Limit when going sideways


            Vector2 gamepadPos = gamepadPosition.ReadValue<Vector2>();
            Vector2 pointerPos = pointerPosition.ReadValue<Vector2>();

            if(InputManager.Instance.LastInputDevice == InputDeviceType.Gamepad)
            {
                Vector3 right = mainCam.transform.right;
                Vector3 forward = mainCam.transform.forward;

                Vector3 dir = right.normalized * gamepadPos.x + forward.normalized * gamepadPos.y;

                if (gamepadPos != Vector2.zero)
                {
                    aimTarget.parent = transform.parent;
                    aimTarget.position = new Vector3(transform.position.x + dir.x, transform.position.y, transform.position.z + dir.z);
                }
                else
                {
                    //move with player
                    aimTarget.parent = transform;
                }
            }
            else if(InputManager.Instance.LastInputDevice == InputDeviceType.MouseAndKeyboard || InputManager.Instance.LastInputDevice == InputDeviceType.Touchscreen)
            {
                Vector2 pointerScreenPosVal = GetPointerValue(pointerPos);

                if (pointerScreenPosVal != Vector2.zero)
                {
                    aimTarget.parent = transform.parent;
                    aimTarget.position = PointerToWorldPos(pointerScreenPosVal);
                }
                else
                {
                    //move with player
                    aimTarget.parent = transform;
                }
            }
            else
            {
                //move with player
                aimTarget.parent = transform;
            }


            if (attack.inProgress)
            {
                Attack();
            }

            if (dodge.triggered)
            {
                Dodge();
            }
        }
    }

    private Vector3 GetPointerValue(Vector2 pointerPos)
    {
        Vector2 targetPointerPos = Vector2.zero;

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
        moveDirection = new Vector3(moveDirection.x, moveDirection.y - gravity, moveDirection.z);

        controller.Move(movementSpeed * Time.deltaTime * moveDirection);


        Vector3 animDirection = transform.InverseTransformDirection(moveDirection);

        anim.SetFloat("WalkX", animDirection.x, AnimationTransitionSpeed, Time.deltaTime);
        anim.SetFloat("WalkY", animDirection.z, AnimationTransitionSpeed, Time.deltaTime);

        GameController.Instance.AudioController.Play("PlayerWalk");
    }

    private void RotatePlayer(Vector3 lookAtPoint)
    {
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
        if (weaponController != null)
        {
            weaponController.Attack(aimTarget.position);
        }
    }

    private void TakeDamage()
    {
        anim.SetTrigger("GetHurt");
    }

    private void Dodge()
    {
        anim.SetTrigger("Roll");

        GameController.Instance.AudioController.Play("PlayerDodge");

        StartCoroutine(DodgingCoroutine(dodgingTime));
    }
    private IEnumerator DodgingCoroutine(float dodgeTime)
    {
        IsDodging = true;

        yield return new WaitForSeconds(dodgeTime);

        IsDodging = false;
    }


    private void OnControllerColliderHit(ControllerColliderHit hit) //Apply player push force on rigidbody objects
    {
        if (rigidbodyForceEnabled)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            if (body == null || body.isKinematic) // If object has no rigidbody
            {
                return;
            }

            if (hit.moveDirection.y < -0.3) // if object is below us
            {
                body.AddForceAtPosition(new Vector3(0, -1.0f, 0) * rigidbodyPushForce, hit.point);
            }
            else
            {
                body.velocity = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z) * rigidbodyPushForce;// Calculate push direction from move direction
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (GameController.Instance != null && GameController.Instance.ShowDebug)
        {
            if (GameController.Instance.IsGameStarted)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(aimTarget.position, 0.25f);
            }
        }
    }

    private void OnGUI()
    {
        if (GameController.Instance.ShowDebug)
        {
            GUI.color = Color.black;

            GUILayout.BeginArea(new Rect(0, (Screen.height / 2) - 25f, 250f, 75f));

            GUILayout.Label($"Player Input: {movementInput}");
            GUILayout.Label($"Player Velocity: {controller.velocity}");
            GUILayout.Label($"IsDodging: {IsDodging}");

            GUILayout.EndArea();
        }
    }
}