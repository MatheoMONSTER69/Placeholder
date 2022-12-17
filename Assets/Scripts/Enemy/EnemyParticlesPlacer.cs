using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyParticlesPlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect vfxRemains;
    [SerializeField] private VisualEffect vfxDeath;
    
    private static Queue<Vector3> positionsQueue = new();

    private float prevRemainsPlayRate = 1;


    private void OnEnable()
    {
        //Invoke("AssignEvents", 0.1f);
    }
    /*private void AssignEvents()
    {
        GameController.Instance.WavesController.OnWaveStart.AddListener(ResumeRemainsParticles);
        GameController.Instance.WavesController.OnWaveFinish.AddListener(PauseRemainsParticles);
    }*/

    private void OnDisable()
    {
        /*GameController.Instance.WavesController.OnWaveStart.RemoveListener(ResumeRemainsParticles);
        GameController.Instance.WavesController.OnWaveFinish.RemoveListener(PauseRemainsParticles);*/
    }

    private void Update()
    {
        if (positionsQueue.Count > 0)
        {
            Vector3 position = positionsQueue.Dequeue();

            SpawnRemainsParticle(position);
            SpawnDeathParticles(position);
        }
    }


    public static void SpawnParticles(Vector3 position)
    {
        positionsQueue.Enqueue(position);
    }


    private void SpawnRemainsParticle(Vector3 position)
    {
        vfxRemains.SetVector3("SpawnPosition", position);

        vfxRemains.SendEvent("SpawnParticle");
    }

    private void SpawnDeathParticles(Vector3 position)
    {
        vfxDeath.SetVector3("SpawnPosition", position);

        vfxDeath.SendEvent("SpawnParticles");
    }


    [ContextMenu("PauseRemains")]
    private void PauseRemainsParticles()
    {
        prevRemainsPlayRate = vfxRemains.playRate;
        vfxRemains.playRate = 0;
    }

    [ContextMenu("ResumeRemains")]
    private void ResumeRemainsParticles()
    {
        vfxRemains.playRate = prevRemainsPlayRate;
    }

    [ContextMenu("CleanRemainsParticles")]
    private void CleanRemainsParticles()
    {
        vfxRemains.Reinit();
    }
}
