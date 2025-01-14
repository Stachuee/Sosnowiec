using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerParticleTrigger : MonoBehaviour
{

    SpawnerController spawnerController;
    [SerializeField] Status toApply;

    private void Start()
    {
        spawnerController = SpawnerController.instance;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.transform.tag == "Enemy")
        {
            spawnerController.GetITakeDamageFormMap(other.transform).ApplyStatus(toApply);
        }
    }
}
