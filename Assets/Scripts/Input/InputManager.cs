using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    [Header("Reference")]
    private InputActionAsset actionMapAsset;
    private PlayerInput playerInput;


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
