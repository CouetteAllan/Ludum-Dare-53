using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Inactive,
    Active,
    ColoredP1,
    ColoredP2
}


public class Building : MonoBehaviour
{
    [System.NonSerialized]
    public State CurrentState = State.Inactive;

    [SerializeField]
    List<GameObject> Spots;

    private void Start()
    {
        foreach(var v in Spots)
        {
            v.GetComponent<Spot>().parentBuilding = this;
            v.SetActive(false);
        }
    }

    public void ChangeState(State newState)
    {
        if (newState == CurrentState)
            return;
        
        switch (newState)
        {
            case State.Active:

                SetActive();
                break;
            case State.ColoredP1:
                
                SetColoredP1();
                break;
            case State.ColoredP2:
                
                SetColoredP2();
                break;
        }
    }

    void SetActive()
    {
        CurrentState = State.Active;
        RandomSpotActive();
    }

    void RandomSpotActive()
    {
        int rand;
        rand = Random.Range(0, Spots.Count);
        Spots[rand].SetActive(true);
    }

    void SetColoredP1()
    {
        //mettre couleur p1p2
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        foreach (var v in Spots)
        {
            v.SetActive(false);
        }
        PackageManager.Instance.SpawnPackage();
        BuildingManager.Instance.ActivateRandomBuilding();
        if (CurrentState == State.ColoredP2)
            GameManager.Instance.ScoreP2--;
        GameManager.Instance.ScoreP1++;

        CurrentState = State.ColoredP1;
    }

    void SetColoredP2()
    {
        //mettre couleur p1p2
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        foreach (var v in Spots)
        {
            v.SetActive(false);
        }
        PackageManager.Instance.SpawnPackage();
        BuildingManager.Instance.ActivateRandomBuilding();
        if(CurrentState == State.ColoredP1)
            GameManager.Instance.ScoreP1--;
        GameManager.Instance.ScoreP2++;

        CurrentState = State.ColoredP2;
    }
}
