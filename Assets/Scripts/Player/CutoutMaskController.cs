using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutMaskController : MonoBehaviour
{
    [SerializeField] private LayerMask maskedLayer;


    private void Start()
    {
        UpdateRendererMaterials();
    }


    [ContextMenu("UpdateRendererMaterials")]
    public void UpdateRendererMaterials()
    {
        Renderer[] maskedObjects = LayerUtils.FindComponentsInLayer<Renderer>(maskedLayer, true);

        for (int i = 0; i < maskedObjects.Length; i++)
        {
            foreach (Material material in maskedObjects[i].materials)
            {
                material.renderQueue = 3002;
            }
        }
    }
}