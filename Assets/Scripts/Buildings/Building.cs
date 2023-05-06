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
    public SpriteRenderer buildingSprite;

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
                SetColorPlayer(playerIndex: 1);
                break;

            case State.ColoredP2:
                SetColorPlayer(playerIndex: 2);
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
        UIManager.Instance.MainCamera = Helpers.Camera;
        UIManager.Instance.AddTargetIndicator(Spots[rand].gameObject);
    }


    private void SetColorPlayer(int playerIndex)
    {
        var newColor = ownedPlayer.CharacterData.spriteColor;

        var spriteEffect = effect.GetComponent<SpriteRenderer>();
        newColor.a = 0.35f;
        spriteEffect.color = newColor;
        foreach (var v in Spots)
        {
            v.SetActive(false);
        }
        PackageManager.Instance.SpawnPackage();
        BuildingManager.Instance.ActivateRandomBuilding();

        #region Change Color for corresponding Player Index
        if (playerIndex == 1)
        {
            if (CurrentState == State.ColoredP2)
                GameManager.Instance.ScoreP2--;
            else if (CurrentState == State.ColoredP1)
                return;
            GameManager.Instance.ScoreP1++;

            CurrentState = State.ColoredP1;
        }
        else
        {
            if (CurrentState == State.ColoredP1)
                GameManager.Instance.ScoreP1--;
            else if (CurrentState == State.ColoredP2)
                return;
            GameManager.Instance.ScoreP2++;

            CurrentState = State.ColoredP2;
        }
        #endregion

        effect.SetActive(true);
        effect.gameObject.GetComponent<Animator>().Play("New Animation", -1, 0f);
        //effect.gameObject.GetComponent<Animator>().SetTrigger("Play");
        StartCoroutine(DelayResetAlpha(spriteEffect));
        if (GameManager.Instance.CurrentState == GameState.DrawState)
        {
            GameManager.Instance.ChangeGameState(GameState.Win);

        }
    }

    private IEnumerator DelayResetAlpha(SpriteRenderer sprite)
    {
        yield return new WaitForSeconds(1.2f);
        var tmp = sprite.color;
        tmp.a = 1.0f;
        buildingSprite.color = tmp;
        tmp.a = 0.0f;
        sprite.color = tmp;
    }

}
