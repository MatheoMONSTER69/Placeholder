using System.Collections.Generic;
using UnityEngine;

public class LayerUtils : MonoBehaviour
{
    public static GameObject[] FindGameObjectsInLayer(LayerMask layerMask)
    {
        var goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        var goList = new List<GameObject>();

        for (int i = 0; i < goArray.Length; i++)
        {
            if (CompareLayers(goArray[i].layer, layerMask))
            {
                goList.Add(goArray[i]);
            }
        }

        return goList.ToArray();
    }

    public static bool CompareLayers(int gameObjectLayer, LayerMask layerMask)
    {
        return (layerMask & 1 << gameObjectLayer) == 1 << gameObjectLayer;
    }
}
