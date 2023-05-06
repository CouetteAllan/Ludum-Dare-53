using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : Singleton<BuildingManager>
{
    [SerializeField]
    private List<Building> buildings = new List<Building>();
    public int BuildingCount { get; private set; }
    private List<Building> buildingsColored = new List<Building>();
    public List<Building> BuildingsColored { get { return buildingsColored; } }
    

    public void ActivateRandomBuilding()
    {
        int rand;
        if(AllBuildingAreColored)
        {
            do
            {
                rand = Random.Range(0, buildingsColored.Count);
            } while (buildingsColored[rand].CurrentState == State.Active);
            buildingsColored[rand].ChangeState(State.Active);
        }
        else
        {
            rand = Random.Range(0, buildings.Count);
            buildings[rand].ChangeState(State.Active);
            buildingsColored.Add(buildings[rand]);
            buildings.RemoveAt(rand);
        }
        
    }


    private void Start()
    {
        BuildingCount = buildings.Count;
        ActivateRandomBuilding();
    }

    private bool AllBuildingAreColored => buildings.Count <= 0;
}
