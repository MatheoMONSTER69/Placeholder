using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    [Header("States")]
    public InputDeviceType LastPointerDevice = InputDeviceType.Unknown;

    [Header("Reference")]
    private InputActionAsset actionMapAsset;
    private PlayerInput playerInput;

    private InputAction mousePointerInput;
    private InputAction gamepadPointerPosition;
    private Vector2 prevMousePointerPos = Vector2.zero;


    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        #region Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        #endregion


        playerInput = GetComponent<PlayerInput>();
		actionMapAsset = playerInput.actions;


        //For pointer detection
        mousePointerInput = GetAction(ActionMapType.Gameplay, InputType.PointerPosition);
        gamepadPointerPosition = GetAction(ActionMapType.Gameplay, InputType.GamepadPosition);
    }

    private void Update()
    {
        CheckPointerInputDevice();
    }


    private void CheckPointerInputDevice()
    {
        Vector2 gamepadPos = gamepadPointerPosition.ReadValue<Vector2>();
        Vector2 mousePos = mousePointerInput.ReadValue<Vector2>();

        //Gamepad
        if (Gamepad.current != null && gamepadPos != Vector2.zero && mousePos == prevMousePointerPos)
        {
            LastPointerDevice = InputDeviceType.Gamepad;
        }
        //Touchscreen
        else if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
        {
            LastPointerDevice = InputDeviceType.Touchscreen;
        }
        //Mouse
        else if (Mouse.current != null && mousePos != prevMousePointerPos)
        {
            prevMousePointerPos = mousePos;

            LastPointerDevice = InputDeviceType.Mouse;
        }
    }

    public InputAction GetAction(ActionMapType actionMap, InputType actionType)
    {
        return actionMapAsset[$"{actionMap}/{actionType}"];
    }

    public void SwitchCurrentActionMap(ActionMapType actionMap)
    {
		foreach (InputAction action in playerInput.currentActionMap.actions)
		{
            action.Disable();
		}

		playerInput.SwitchCurrentActionMap($"{actionMap}");

        foreach(InputAction action in playerInput.currentActionMap.actions)
        {
            action.Enable();
		}
    }
}
