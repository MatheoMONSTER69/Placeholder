using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCam;

    [Header("References")]
    [SerializeField] private Transform cameraCutoutMask;

    [Header("Settings")]
    [SerializeField] private Vector2 cutoutOffset = new Vector2(0.0f, 0.0f);
    [SerializeField] private Vector2 cutoutSize = new Vector2(0.05f, 0.05f);

    private void Start()
    {
        mainCam = Camera.main;

        SetCameraCutoutMaskSize();
    }


    private void SetCameraCutoutMaskSize()
    {
        cameraCutoutMask.localPosition = new Vector3(cutoutOffset.x, cutoutOffset.y, mainCam.farClipPlane + mainCam.nearClipPlane + 0.001f);
        cameraCutoutMask.localScale = new Vector3(cutoutSize.x, mainCam.farClipPlane, cutoutSize.y);
    }
}
