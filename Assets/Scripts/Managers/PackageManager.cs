using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageManager : Singleton<PackageManager>
{
    public List<GameObject> spawns;
    [SerializeField]
    GameObject packagePrefab;

    GameObject currentPackage;

    [SerializeField] float timerDelaySpawnPackage;
    

    public void SpawnPackage()
    {
        if(!currentPackage)
        currentPackage = Instantiate(packagePrefab,spawns[0].transform);
    }

    private void Start()
    {
        SpawnPackage();
    }

}
