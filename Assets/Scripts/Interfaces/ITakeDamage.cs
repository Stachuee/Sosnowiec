using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DamageEffetcts { None, Disintegrating }
public enum DamageSource { Player, Turret, Enemy}
public interface ITakeDamage 
{
    public float TakeDamage(float damage, DamageSource source, DamageEffetcts effects = DamageEffetcts.None);
    public float Heal(float ammount);
    public void ApplyStatus(Status toApply);
    public void TakeArmorDamage(float damage);
    public bool IsImmune();
    public bool IsArmored();
    public Transform GetTransform();
}

public interface ITakeDamageInteractable : ITakeDamage
{
    public bool AlwaysAttack();
}
