using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VisionMaterialData",menuName = "ScriptableObjects/CreateVisionMaterialData")]
public class VisionMaterialData : ScriptableObject
{
    public Material dangerMaterial;
    public Material safeMaterial;
    public Material detectedMaterial;
}
