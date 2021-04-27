using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public interface IInteractive
{
    void Interact(InteractionType interactionType);
    InteractionType GetInteractionType();
    Vector3 GetPosition();
    OutfitData GetDisguiseOutfit();
}

public enum InteractionType
{
    Kill,
    Pickup,
    Use,
    Disguise,
    Locked,
}