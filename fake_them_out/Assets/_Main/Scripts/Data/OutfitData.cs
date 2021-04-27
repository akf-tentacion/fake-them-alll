using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OutfitData", menuName = "ScriptableObjects/OutfitData")]
public class OutfitData : ScriptableObject
{
    public string Name;
    public Outfit Outfit;
    public OutfitType Type;
}