using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDB", menuName = "Scriptable Objects/CharacterDB")]
public class CharacterDB : ScriptableObject
{
    public List<GameObject> charDB;
}
