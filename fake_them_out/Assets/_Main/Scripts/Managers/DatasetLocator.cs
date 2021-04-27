using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AKUtil;

public class DatasetLocator : SingletonMonoBehaviour<DatasetLocator>
{
    [SerializeField] VisionMaterialData visionMaterialData;
    [SerializeField] OutfitData nakedOutfitData;

    public VisionMaterialData VisionMaterialData { get { return visionMaterialData; } }
    public OutfitData NakedOutfitData { get { return nakedOutfitData; } }
}
