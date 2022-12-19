using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public bool IsShot = false;

    private float Speed;
    private Cooldown DisablingCooldown;
    private bool DetonateAfterCooldown;
    private LayerMask CollisionLayer;
    private UnityAction<Vector3, GameObject> DetonateAction;


    private void Update()
    {
        MoveProjectile();
    }


    public void Init(float speed, LayerMask collisionLayer, UnityAction<Vector3, GameObject> detonateAction, float disablingCooldown = 5.0f, bool detonateAfterCooldown = false, bool startAfterInit = false)
    {
        Speed = speed;
        DetonateAction = detonateAction;
        CollisionLayer = collisionLayer;

        DisablingCooldown = new(disablingCooldown);
        DetonateAfterCooldown= detonateAfterCooldown;

        if(startAfterInit)
        {
            StartProjectile();
        }
    }

    public void StartProjectile()
    {
        DisablingCooldown.StartCooldown();
        IsShot = true;
    }

    private void MoveProjectile()
    {
        if(IsShot)
        {
            transform.position += Speed * Time.deltaTime * -transform.forward;

            if (!DisablingCooldown.IsInCooldown)
            {
                if(DetonateAfterCooldown)
                {
                    DetonateProjectile(transform.position, null);
                }
                else
                {
                    DisableProjectile();
                }

                return;
            }

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1.0f, CollisionLayer))
            {
                DetonateProjectile(transform.position, hit.collider.gameObject);
            }
        }
    }

    public void DetonateProjectile(Vector3 position, GameObject col)
    {
        DetonateAction.Invoke(position, col);

        DisableProjectile();
    }

    public void DisableProjectile()
    {
        Destroy(gameObject);
    }
}
