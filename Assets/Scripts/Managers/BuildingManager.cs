using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : Singleton<BuildingManager>
{
    [SerializeField]
    List<Building> buildings;

    public void ActivateRandomBuilding()
    {
        int rand;
        do {
            rand = Random.Range(0, buildings.Count);
        } while (buildings[rand].CurrentState != State.Inactive);
        buildings[rand].ChangeState(State.Active);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ActivateRandomBuilding();
        }
    }
}
