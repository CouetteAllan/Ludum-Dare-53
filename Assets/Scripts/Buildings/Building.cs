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

    [SerializeField] private SpriteRenderer graph;

    private PlayerScript ownedPlayer;

    public GameObject effect;
    public SpriteRenderer filter;

    private void Start()
    {
        foreach(var v in Spots)
        {
            v.GetComponent<Spot>().parentBuilding = this;
            //v.SetActive(false);
        }
    }

    public void ScorePlayer(PlayerScript player)
    {
        ownedPlayer = player;
        if (ownedPlayer.PlayerIndex == 0) //Player 1
            ChangeState(State.ColoredP1);
        else if (ownedPlayer.PlayerIndex == 1) ChangeState(State.ColoredP2); //Player 2
        else Debug.Log("Invalid Player index");
    }

    public void ChangeState(State newState)
    {
        if (newState == CurrentState)
            return;
        
        switch (newState)
        {
            case State.Active:
                SetSpotActive();
                break;

            case State.ColoredP1:
                SetColoredP1();
                break;

            case State.ColoredP2:
                SetColoredP2();
                break;
        }
    }

    void SetSpotActive()
    {
        CurrentState = State.Active;
        RandomSpotActive();
    }

    void RandomSpotActive()
    {
        int rand;
        rand = Random.Range(0, Spots.Count);
        Spots[rand].SetActive(true);
        UIManager.Instance.MainCamera = Camera.main;
        UIManager.Instance.AddTargetIndicator(Spots[rand].gameObject);
    }

    void SetColoredP1()
    {
        //mettre couleur p1p2
        var newColor = ownedPlayer.CharacterData.spriteColor;
       // newColor.a = 1;
        //graph.color = newColor;
        newColor.a = 0.35f;
        var tmp = effect.GetComponent<SpriteRenderer>().color;
        tmp.a = 0.35f;
        effect.GetComponent<SpriteRenderer>().color = tmp;
        filter.color = newColor;
        foreach (var v in Spots)
        {
            v.SetActive(false);
        }
        PackageManager.Instance.SpawnPackage();
        BuildingManager.Instance.ActivateRandomBuilding();
        if (CurrentState == State.ColoredP2)
            GameManager.Instance.ScoreP2--;
        else if (CurrentState == State.ColoredP1)
            return;
        GameManager.Instance.ScoreP1++;

        CurrentState = State.ColoredP1;
        effect.SetActive(true);
        effect.gameObject.GetComponent<Animator>().Play("New Animation",-1,0f);
        //effect.gameObject.GetComponent<Animator>().SetTrigger("Play");
    }

    void SetColoredP2()
    {
        //mettre couleur p1p2
        var newColor = ownedPlayer.CharacterData.spriteColor;
       // newColor.a = 1;
        //graph.color = newColor;
        newColor.a = 0.35f;
        var tmp = effect.GetComponent<SpriteRenderer>().color;
        tmp.a = 0.35f;
        effect.GetComponent<SpriteRenderer>().color = tmp;
        filter.color = newColor;
        foreach (var v in Spots)
        {
            v.SetActive(false);
        }
        PackageManager.Instance.SpawnPackage();
        BuildingManager.Instance.ActivateRandomBuilding();
        if(CurrentState == State.ColoredP1)
            GameManager.Instance.ScoreP1--;
        else if (CurrentState == State.ColoredP2)
            return;
        GameManager.Instance.ScoreP2++;

        CurrentState = State.ColoredP2;
        effect.SetActive(true);
        effect.gameObject.GetComponent<Animator>().Play("New Animation", -1, 0f);
        //effect.gameObject.GetComponent<Animator>().GetComponent<Animator>().SetTrigger("Play");
    }
}
