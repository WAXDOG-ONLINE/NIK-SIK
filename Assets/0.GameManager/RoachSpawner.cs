using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RoachSpawner : MonoBehaviour
{
    [SerializeField ]
    private bool enabledSpawn = true;
    [SerializeField ]
    private GameObject _enemyPrefab;
     [SerializeField ]
    private float _minimumSpawnTime;
     [SerializeField ]
    private float maximumSpawnTime;

    private float _timeUntilSpawn;
    // Start is called before the first frame update
    void Awake()
    {
        SetTimeUntilSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        _timeUntilSpawn -= Time.deltaTime;
        if(_timeUntilSpawn <= 0){
            if(enabledSpawn){
            if(GlobalVariables.enemies < GlobalVariables.enemiesMax){    
            GlobalVariables.enemies++;
            Instantiate(_enemyPrefab, transform.position,   Quaternion.identity);
            }
            }
            SetTimeUntilSpawn();
        }
    }


    private void SetTimeUntilSpawn(){


        _timeUntilSpawn = Random.Range(_minimumSpawnTime,maximumSpawnTime);
    }
}
