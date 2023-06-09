using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character",menuName = "Character")]
public class PlayerCharacter : ScriptableObject
{
    public Color spriteColor = Color.black;
    public RuntimeAnimatorController animController;
    public CharacterType characterType;
    public Sprite art;
}

public struct SelectedCharacterData
{
    public Color spriteColor;
    public RuntimeAnimatorController animController;
    public CharacterType characterType;
}

public enum CharacterType
{
    Potito,
    Potita,
    James
}

public enum Chroma
{
    Blue,
    Orange,
    Yellow,
    Pink,
    Green
}