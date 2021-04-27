using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutfitLimitedInteraction : MonoBehaviour, IInteractive
{
    [SerializeField] OutfitType targetOutfit;

    public Action OnInteracted;
    public InteractionType GetInteractionType()
    {
        var playerOutfit = GameRuleManager.Instance.Player.CurrentOutfitType;
        if (playerOutfit != targetOutfit) return InteractionType.Locked;
        return InteractionType.Use;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public OutfitData GetDisguiseOutfit()
    {
        return null;
    }

    public void Interact(InteractionType interactionType)
    {
        OnInteracted?.Invoke();
    }
}
