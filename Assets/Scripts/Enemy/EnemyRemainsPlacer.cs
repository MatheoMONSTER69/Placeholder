using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyRemainsPlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect particles;

    private static Queue<Vector3> positionsQueue = new();

    [Header("Settings")]
    [SerializeField] private bool cleanOnWaveStart = false;
    [SerializeField] private bool cleanOnWaveFinish = false;


    private void OnEnable()
    {
        if(cleanOnWaveStart)
        {
            GameController.Instance.WavesController.OnWaveStart.AddListener(CleanParticles);
        }
        
        if(cleanOnWaveFinish)
        {
            GameController.Instance.WavesController.OnWaveFinish.AddListener(CleanParticles);
        }
    }
    private void OnDisable()
    {
        if (cleanOnWaveStart)
        {
            GameController.Instance.WavesController.OnWaveStart.RemoveListener(CleanParticles);
        }

        if (cleanOnWaveFinish)
        {
            GameController.Instance.WavesController.OnWaveFinish.RemoveListener(CleanParticles);
        }
    }

    private void Update()
    {
        if(positionsQueue.Count > 0)
        {
            SpawnParticle(positionsQueue.Dequeue());
        }
    }


    public static void PlaceRemains(Vector3 position)
    {
        positionsQueue.Enqueue(position);
    }

    private void SpawnParticle(Vector3 position)
    {
        particles.SetVector3("SpawnPosition", position);

        particles.SendEvent("SpawnParticle");
    }

    [ContextMenu("CleanParticles")]
    private void CleanParticles()
    {
        particles.Reinit();
    }
}
