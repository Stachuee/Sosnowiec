using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : InteractableBase, DangerLevelIncrease
{

    protected readonly float PACING_LOCK = 5f;
    public bool isActive = false;

    [SerializeField] bool activeFromStart;
    
    [SerializeField] protected float targetPacing;
    [SerializeField] protected float currentPacing;
    [SerializeField] protected float pacingFalloff;

    [SerializeField] protected float spawnDelay;

    protected float nextWave;

    protected override void Awake()
    {
        base.Awake();
        isActive = false;
    }

    private void Start()
    {
        SpawnerController.instance.AddSpawner(this);
        PacingController.pacingController.AddToNotify(this);
        StartCoroutine("Spawn");
    }

    private void Update()
    {
        currentPacing -= (pacingFalloff / 60) * Time.deltaTime;
        //if (spawning && isActive && lastSpawn + spawnDelay < Time.time)
        //{
        //    if (toSpawnList[step].count > alreadySpawned)
        //    {
        //        GameObject temp = Instantiate(toSpawnList[step].enemy.GetPrefab(), transform.position, Quaternion.identity);
        //        lastSpawn = Time.time;
        //        alreadySpawned++;
        //    }
        //    else if(step < toSpawnList.Count - 1)
        //    {
        //        step++;
        //        spawnDelay = toSpawnList[step].spawnDelay;
        //        alreadySpawned = 0;
        //    }
        //    else
        //    {
        //        spawning = false;
        //        nextWave = Time.time + spawnCooldown;
        //    }
        //}
        //if(nextWave < Time.time)
        //{
        //    GetNewWave();
        //}
    }

    protected virtual IEnumerator Spawn()
    {
        while(true)
        {
            if(isActive && PacingController.wave && currentPacing < targetPacing)
            {
                SpawnEnemy(SpawnerController.instance.GetEnemy(true));
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void SpawnEnemy(EnemySO enemy)
    {
        if(enemy != null)
        {
            currentPacing += enemy.GetWeigth();
            Instantiate(enemy.GetPrefab(), transform.position, Quaternion.identity);
        }
    }

    public virtual void IncreaseLevel(int level)
    {
        if(!isActive && level >= 0 && activeFromStart)
        { 
            isActive = true;
        }
        pacingFalloff = PacingController.currentDangerLevel.GetPacingFallof();
        targetPacing = PacingController.currentDangerLevel.GetTargetPacing();
        currentPacing = targetPacing;
    }

    public virtual void ActivateSpanwer()
    {
        isActive = true;
    }

    public virtual void DeactivateSpawner()
    {
        isActive = false;
    }


    public virtual void GetNewWave()
    {
        //WaveSO temp = SpawnerController.instance.SpawnWave();
        //if (temp != null) AddToSpawn(temp.GetEnemySpawn(), temp.GetNextWaveDelay());
        //else nextWave = Time.time + PACING_LOCK;
    }


}
