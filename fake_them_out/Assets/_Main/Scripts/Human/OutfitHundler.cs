using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// Outfit、外見を変更するクラス
/// </summary>
public class OutfitHundler : MonoBehaviour
{
    [SerializeField] Transform outfitParent;

    private ReactiveProperty<OutfitData> rpCurrentOutfit = new ReactiveProperty<OutfitData>();
    public OutfitData CurrentOutfit { private set { rpCurrentOutfit.Value = value; } get { return rpCurrentOutfit.Value; } }
    public IObservable<OutfitData> OnOutfitChanged { get { return rpCurrentOutfit; } }

    Outfit outfit;
    RelationshipHundler relationshipHundler;
    bool isPlayer = false;

    public Animator Animator { get { return outfit.Animator; } }

    public bool IsNaked
    {
        get { return CurrentOutfit.Type == OutfitType.Naked; }
    }

    public void Initialize(OutfitData outfit, RelationshipHundler relationshipHundler)
    {
        SetOutfit(outfit);
        this.relationshipHundler = relationshipHundler;
        ChangeColorImmediately();
        relationshipHundler.OnRelationshipChanged
            .Subscribe(_ => ChangeColorImmediately());
    }

    void ChangeColorImmediately()
    {
        var color = GetColor(relationshipHundler.CurrentRelationshop);
        SetOutline(color);
    }

    void SetOutline(Color color)
    {
        foreach (var renderer in outfit.Renderers)
        {
            foreach (var mat in renderer.materials)
            {
                mat.SetColor("_OutlineColor", color);
            }
        }
    }

    void SetOutlineWhite()
    {
        SetOutline(Color.white);
    }

    Color GetColor(RelationshipType type)
    {
        switch (type)
        {
            case RelationshipType.Hostile:
                return Color.red;
            case RelationshipType.Friendly:
                return Color.green;
            default:
                return Color.green;
        }
    }

    public void SetOutfit(OutfitData next, bool isPlayer = false)
    {
        if(isPlayer)this.isPlayer = true;
        CurrentOutfit = next;

        if(outfitParent.childCount >= 1)
        {
            DestroyImmediate(outfitParent.GetChild(0).gameObject);
        }

        outfit = Instantiate(CurrentOutfit.Outfit, outfitParent);
        outfit.transform.localPosition = Vector3.zero;
        outfit.transform.localRotation = Quaternion.identity;
        if (this.isPlayer) SetOutlineWhite();
    }

    public void BeNaked()
    {
        SetOutfit(DatasetLocator.Instance.NakedOutfitData);
    }
}
