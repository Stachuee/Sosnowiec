using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : GunBase
{

    [SerializeField] float fireRate = 0.1f;
    private float shootTimer = 0f; // time elapsed since last shot

    [SerializeField] float laserDistance = 100f;
    [SerializeField] Transform firePoint;
    [SerializeField] LineRenderer lineRenderer;

    [SerializeField] LayerMask maskToIgnore;

    [SerializeField] float damage = 5f;
    bool ableToDamage = true;

    public override void Reload()
    {
        currentAmmo = maxAmmo;
    }


    void Update()
    {

        if (isShooting >= 0.9f)
        {
            if (Time.time < shootTimer + fireRate)
            {
                ableToDamage = false;
            }
            else ableToDamage = true;
            lineRenderer.enabled = true;

            if (Physics2D.Raycast(transform.position, transform.right))
            {
                RaycastHit2D hit = Physics2D.Raycast(firePoint.position, transform.right, Mathf.Infinity, ~maskToIgnore);
                DrawRay(firePoint.position, hit.point);
                if (hit.collider != null && ableToDamage)
                {
                    ITakeDamage target = hit.transform.GetComponent<ITakeDamage>();

                    if (target != null) target.TakeDamage(damage, DamageType.Bullet);
                    shootTimer = Time.time;
                }
            }
        }
        else lineRenderer.enabled = false;
    }


    void DrawRay(Vector2 startPosition, Vector2 endPosition)
    {
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

}