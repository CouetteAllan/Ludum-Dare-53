using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character",menuName = "Character")]
public class PlayerCharacter : ScriptableObject
{
    public Color spriteColor = Color.black;
    public AnimatorController animController;
    public CharacterType characterType;
    public Sprite art;
}

public enum CharacterType
{
    Potito,
    Potita,
    James
}