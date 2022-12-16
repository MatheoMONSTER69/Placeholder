using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyRemainsPlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect particles;

    private static Queue<Vector3> positionsQueue = new();

    private float prevPlayRate = 1;


    private void OnEnable()
    {
        Invoke("AssignEvents", 0.1f);
    }
    private void AssignEvents()
    {
        GameController.Instance.WavesController.OnWaveStart.AddListener(ResumeParticles);
        GameController.Instance.WavesController.OnWaveFinish.AddListener(PauseParticles);
    }

    private void OnDisable()
    {
        GameController.Instance.WavesController.OnWaveStart.RemoveListener(ResumeParticles);
        GameController.Instance.WavesController.OnWaveFinish.RemoveListener(PauseParticles);
    }

    private void Update()
    {
        if (positionsQueue.Count > 0)
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

    [ContextMenu("Pause")]
    private void PauseParticles()
    {
        prevPlayRate = particles.playRate;
        particles.playRate = 0;
    }

    [ContextMenu("Resume")]
    private void ResumeParticles()
    {
        particles.playRate = prevPlayRate;
    }

    [ContextMenu("CleanParticles")]
    private void CleanParticles()
    {
        particles.Reinit();
    }
}
