using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molotov : NadeBase, ITakeDamage
{
    [SerializeField] float fireDuration;

    [SerializeField] GameObject fireSFX;

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        FireExplode((Vector2)transform.position + collision.GetContact(0).normal * 0.5f);
    }

    public void FireExplode(Vector2 firePosition)
    {
        Explode();
        var sfx = Instantiate(fireSFX);
        Destroy(sfx, fireDuration);
        Destroy(Instantiate(explosionVFX, firePosition, Quaternion.identity), fireDuration);
        Destroy(gameObject);
    }

    public float TakeDamage(float damage, DamageSource source, DamageEffetcts effects = DamageEffetcts.None)
    {
        FireExplode(transform.position);
        return 0;
    }

    public void ApplyStatus(Status toApply)
    {
        
    }

    public void TakeArmorDamage(float damage)
    {
        
    }

    public bool IsImmune()
    {
        return false;
    }

    public bool IsArmored()
    {
        return false;
    }
    public float Heal(float ammount)
    {
        return 0;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
