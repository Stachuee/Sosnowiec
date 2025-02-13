using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocade : MonoBehaviour, ITakeDamage
{

    [SerializeField]
    float maxHp;
    [SerializeField]
    float hp;
    [SerializeField] string displayName;
    [SerializeField] DoorScript door;

    [SerializeField]
    GameObject bossToSpawn;
    bool bossSpawned;

    void Start()
    {
        hp = maxHp;
    }

    public float TakeDamage(float damage, DamageSource source, DamageEffetcts effects = DamageEffetcts.None)
    {
        float damageAmmount = damage;
        //if (type == DamageType.Fire)
        //{
        //    damage *= 1.5f;
        //}
        hp -= damageAmmount;
        if (hp < 0)
        {
            Destroy(gameObject);
        }
        else if (hp / maxHp < 0.25f && !bossSpawned)
        {
            bossSpawned = true;
            Instantiate(bossToSpawn, transform.position, Quaternion.identity);
        }
        return damageAmmount;
    }

    public ComputerInfoContainer GetInfo()
    {
        return new ComputerInfoContainer { hp = hp, maxHp = maxHp, showHp = true, name = displayName };
    }

    public void TakeArmorDamage(float damage)
    {
        
    }

    public bool IsImmune()
    {
        return false;
    }

    public void ApplyStatus(Status toApply)
    {
        
    }


    public bool IsArmored()
    {
        return false;
    }

    public float Heal(float ammount)
    {
        hp = Mathf.Min(ammount + hp, maxHp);
        return ammount;
    }
    public Transform GetTransform()
    {
        return transform;
    }
}
