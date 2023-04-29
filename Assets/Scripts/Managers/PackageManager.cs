using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageManager : Singleton<PackageManager>
{
    public List<GameObject> spawns;
    [SerializeField]
    GameObject packagePrefab;

    GameObject currentPackage;

    private void Start()
    {
        SpawnPackage();
    }

    public void SpawnPackage()
    {
        if (currentPackage)
            Destroy(currentPackage);
        currentPackage = Instantiate(packagePrefab,spawns[0].transform);
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnPackage();
        }
    }*/
}
