using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSoundPlay : MonoBehaviour
{
    public void Init()
    {
        PlayerScript.OnPlayerScore += PlayerScript_OnPlayerScore;
    }

    private void PlayerScript_OnPlayerScore(int playerIndex)
    {
        SoundManager.Instance.Play("Score");
    }

    private void OnDisable()
    {
        PlayerScript.OnPlayerScore -= PlayerScript_OnPlayerScore;
    }
}
