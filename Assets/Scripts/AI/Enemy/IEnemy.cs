using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    void Move(Vector3 targetPosition);
    //void Attack();
    void TakeDamage(int damageAmount);
    void Die();
}