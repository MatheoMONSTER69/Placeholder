using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantRoomController : MonoBehaviour
{
    /// <summary>
    /// Triggers intersection layout required for entering/leaving detection:
    /// 
    ///      ----------
    /// -----|--Door--|-----
    /// |    ----------    |
    /// |                  |
    /// |       Room       |
    /// |                  |
    /// --------------------
    /// 
    /// </summary>


    [SerializeField] private MerchantController merchantController;

    [SerializeField] private GameObject merchantRoomDoors;

    [SerializeField] private Trigger roomTrigger; //Has to extend to half of door collider
    [SerializeField] private Trigger doorTrigger; //Has to half-intersect with roomTrigger

    [HideInInspector] public bool IsPlayerInMerchantRoom = false;
    [HideInInspector] public bool IsDoorOpen = false;
    private bool hasPlayerFinishedMerchantInteraction = false;


    private void OnEnable()
    {
        doorTrigger.OnTriggerEnterEvent.AddListener(DoorTriggerEnter);
        doorTrigger.OnTriggerExitEvent.AddListener(DoorTriggerExit);

        roomTrigger.OnTriggerEnterEvent.AddListener(RoomTriggerEnter);
        roomTrigger.OnTriggerExitEvent.AddListener(RoomTriggerExit);

        merchantController.OnInteractionFinish.AddListener(FinishedMerchantInteraction);

        Invoke("AddSingletonListeners", 0.1f);
    }
    private void AddSingletonListeners()
    {
        GameController.Instance.WavesController.OnWaveFinish.AddListener(OpenMerchant);
    }

    private void OnDisable()
    {
        doorTrigger.OnTriggerEnterEvent.RemoveListener(DoorTriggerEnter);
        doorTrigger.OnTriggerExitEvent.RemoveListener(DoorTriggerExit);

        roomTrigger.OnTriggerEnterEvent.RemoveListener(RoomTriggerEnter);
        roomTrigger.OnTriggerExitEvent.RemoveListener(RoomTriggerExit);

        merchantController.OnInteractionFinish.RemoveListener(FinishedMerchantInteraction);

        GameController.Instance.WavesController.OnWaveFinish.RemoveListener(OpenMerchant);
    }


    private void DoorTriggerEnter(GameObject go)
    {
        //Debug.Log("Entered door");
    }
    private void DoorTriggerExit(GameObject go)
    {
        if(IsPlayerInMerchantRoom)
        {
            if(!hasPlayerFinishedMerchantInteraction)
            {
                FullyEnteredRoom();
            }
            else
            {
                CanceledLeavingRoom();
            }
        }
        else
        {
            if (hasPlayerFinishedMerchantInteraction)
            {
                FullyLeavedRoom();
            }
            else
            {
                CanceledEnteringRoom();
            }
        }
    }

    private void RoomTriggerEnter(GameObject go)
    {
        IsPlayerInMerchantRoom = true;

    }
    private void RoomTriggerExit(GameObject go)
    {
        IsPlayerInMerchantRoom = false;
    }


    private void CanceledEnteringRoom()
    {
        //Debug.Log("Left merchant room door without fully entering merchant room");
    }

    private void FullyEnteredRoom()
    {
        //Debug.Log("Entered merchant room from outside");

        CloseRoom();
    }

    private void CanceledLeavingRoom()
    {
        //Debug.Log("Left merchant room door without fully leaving the room");
    }

    private void FullyLeavedRoom()
    {
        //Debug.Log("Left merchant room from inside");

        CloseMerchant();
    }


    [ContextMenu("OpenMerchant")]
    public void OpenMerchant()
    {
        Cleanup();

        OpenRoom();
    }
    [ContextMenu("CloseMerchant")]
    public void CloseMerchant()
    {
        CloseRoom();

        Cleanup();

        GameController.Instance.WavesController.StartNextWave();
    }


    [ContextMenu("OpenRoom")]
    public void OpenRoom()
    {
        merchantRoomDoors.SetActive(false);

        IsDoorOpen = true;
    }
    [ContextMenu("CloseRoom")]
    public void CloseRoom()
    {
        merchantRoomDoors.SetActive(true);

        IsDoorOpen = false;
    }


    private void FinishedMerchantInteraction()
    {
        hasPlayerFinishedMerchantInteraction = true;

        OpenRoom();
    }

    private void Cleanup()
    {
        hasPlayerFinishedMerchantInteraction = false;
    }
}
