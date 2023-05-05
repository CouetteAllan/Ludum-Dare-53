using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndPanelScript : MonoBehaviour
{
    public event Action OnEndAnimEvent;
    [SerializeField] private Sprite[] splashes;
    private const int POTITA_INDEX = 0;
    private const int POTITO_INDEX = 1;

    [Space(2.0f)]
    [SerializeField] private Image splashImage;
    [SerializeField] private Sprite[] playerIndexWinSprites;
    [SerializeField] private Image playerIndexWin;

    public void SetWinnerSplashAndText(int winnerIndex, SelectedCharacterData selectedCharacter)
    {
        playerIndexWin.sprite = playerIndexWinSprites[winnerIndex];
        switch (selectedCharacter.characterType)
        {
            case CharacterType.Potito:
                splashImage.sprite = splashes[POTITO_INDEX];
                break;
            case CharacterType.Potita:
                splashImage.sprite = splashes[POTITA_INDEX];
                break;
            case CharacterType.James:
                break;
        }

        splashImage.color = selectedCharacter.spriteColor;
    }

    public void StartEndAnim()
    {
        this.GetComponent<Animator>().SetTrigger("Play");
    }


    public void OnEndAnim()
    {
        OnEndAnimEvent?.Invoke();
    }
}
