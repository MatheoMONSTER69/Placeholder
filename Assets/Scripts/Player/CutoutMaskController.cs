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
        GameObject[] maskedObjects = LayerUtils.FindGameObjectsInLayer(maskedLayer);

        for (int i = 0; i < maskedObjects.Length; i++)
        {
            if(maskedObjects[i].TryGetComponent(out Renderer renderer))
            {
                foreach (Material material in renderer.materials)
                {
                    material.renderQueue = 3002;
                }
            }          
        }
    }
}

