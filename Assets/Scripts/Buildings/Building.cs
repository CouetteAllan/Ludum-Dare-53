using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Inactive,
    Active,
    Colored
}


public class Building : MonoBehaviour
{
    public State CurrentState = State.Inactive;

    /*[SerializeField]
    List<>*/

    public void ChangeState(State newState)
    {
        if (newState == CurrentState)
            return;
        
        switch (newState)
        {
            case State.Active:
                SetActive();
                break;
            case State.Colored:
                if(CurrentState == State.Active)
                    SetColored();
                break;
        }
    }

    void SetActive()
    {
        CurrentState = State.Active;
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
    }

    void SetColored()
    {
        CurrentState = State.Colored;
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
    }
}
