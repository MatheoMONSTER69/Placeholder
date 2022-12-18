using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MerchantController : MonoBehaviour
{
    [SerializeField] private MerchantRoomController merchantRoomController;

    [HideInInspector] public bool HasPlayerPickedUpgrade = false;

    private InputAction merchantInteraction;

    public UnityEvent OnInteractionStart;
    public UnityEvent OnPickedUpgrade;
    public UnityEvent OnInteractionFinish;


    private void Awake()
    {
        merchantInteraction = InputManager.Instance.GetAction(ActionMapType.Gameplay, InputType.MerchantInteraction);
    }

    private void OnEnable()
    {
        //Invoke("AddSingletonListeners", 0.1f);
    }
    /*private void AddSingletonListeners()
    {
        //GameController.Instance.
    }*/

    private void OnDisable()
    {
        //GameController.Instance.
    }

    private void Update()
    {
        if(merchantRoomController.IsPlayerInMerchantRoom && !merchantRoomController.IsDoorOpen)
        {
            if (merchantInteraction.triggered)
            {
                StartInteraction();
            }
        }
    }


    //Launched from bind
    [ContextMenu("StartMerchantInteraction")]
    public void StartInteraction()
    {
        //Debug.Log("Started merchant interaction");


        //TODO: Show UI etc.


        OnInteractionStart.Invoke();


        PickUpgrade(); //TEMP, until there's no UI
    }

    //Launched after finished everything in UI
    [ContextMenu("FinishMerchantInteraction")]
    public void FinishInteraction()
    {
        //Debug.Log("Finished merchant interaction");


        //TODO: Hide UI etc.


        OnInteractionFinish.Invoke();
    }

    //Launched from picking upgrade in UI
    [ContextMenu("PickUpgrade")]
    public void PickUpgrade()
    {
        //Debug.Log("Picked upgrade");

        HasPlayerPickedUpgrade = true;


        //TODO: Save picked upgrade from UI etc.


        OnPickedUpgrade.Invoke();


        FinishInteraction(); //TEMP, until there's no UI
    }


    private void OnGUI()
    {
        if (GameController.Instance.ShowDebug)
        {
            GUI.color = Color.black;

            GUILayout.BeginArea(new Rect((Screen.width / 2) - 125f, 0, 250f, 100f));

            GUILayout.Label($"IsPlayerInMerchantRoom: {merchantRoomController.IsPlayerInMerchantRoom}");
            GUILayout.Label($"IsMerchantDoorOpen: {merchantRoomController.IsDoorOpen}");
            GUILayout.Label($"HasPlayerPickedUpgrade: {HasPlayerPickedUpgrade}");

            GUILayout.EndArea();
        }
    }
}
