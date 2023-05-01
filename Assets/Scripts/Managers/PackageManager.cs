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
        if(currentPackage == null)
            currentPackage = Instantiate(packagePrefab,spawns[0].transform);
        StartCoroutine(DelaySpawn(timerDelaySpawnPackage));
    }

    private IEnumerator DelaySpawn(float delay)
    {
        //Faire des effets ? un chrono qui s'affiche au dessus du spawn d'objet ?
        currentPackage.SetActive(false);
        yield return new WaitForSeconds(delay);
        currentPackage.transform.position = spawns[0].transform.position;
        currentPackage.SetActive(true);
    }

    private void Start()
    {
        PlayerScript.OnPlayerScore += PlayerScript_OnPlayerScore;
        SpawnPackage();
    }

    private void PlayerScript_OnPlayerScore(int playerIndex)
    {
        //Zwosh effect à l'endroit du package pour dire qu'il a disparu ? anim de victoire ?
        SpawnPackage();
    }

    private void OnDisable()
    {
        PlayerScript.OnPlayerScore -= PlayerScript_OnPlayerScore;
    }
}
