using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectFaceCamera))]
public class CutoutObjectController : MonoBehaviour
{
    private Camera mainCam;

    [Header("Settings")]
    [SerializeField] private Vector2 cutoutOffset = new Vector2(0.0f, 0.0f);
    [SerializeField] private Vector2 cutoutSize = new Vector2(0.02f, 0.02f);
    [SerializeField] private bool update = true;


    private void Start()
    {
        mainCam = Camera.main;

        SetCutoutObjectSize();
    }

    private void Update()
    {
        if(update)
        {
            SetCutoutObjectSize();
        }
    }


    private void SetCutoutObjectSize()
    {
        transform.localPosition = new Vector3(cutoutOffset.x, cutoutOffset.y, transform.localPosition.z);

        //float dist = Vector3.Distance(mainCam.transform.position, transform.position);

        transform.localScale = new Vector3(cutoutSize.x, /*dist - 0.5f*/ mainCam.farClipPlane, cutoutSize.y);
    }
}
