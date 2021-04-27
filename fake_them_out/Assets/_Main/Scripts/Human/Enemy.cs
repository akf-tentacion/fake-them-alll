using System;
using UniRx;
using UnityEngine;

public class Enemy : MonoBehaviour, IInteractive
{
    [SerializeField] AIController ai;
    [SerializeField] CharacterDetector characterDetector;
    [SerializeField] OutfitHundler outfitHundler;
    [SerializeField] HumanController humanController;
     
    public InteractionType CurrentInteraction { private set; get; }

    void Start()
    {
        characterDetector.OnLost.Subscribe(_ => UpdateInteractionType());
        characterDetector.OnDetect.Subscribe(_ => UpdateInteractionType());
        ai.OnStateChanged.Subscribe(_ => UpdateInteractionType());
        outfitHundler.OnOutfitChanged.Subscribe(_ => UpdateInteractionType());
    }

    void UpdateInteractionType()
    {
        //敵が死んでいる かつ 洋服をきてる ならば変装可能。
        bool canDisguise = ai.IsDead() && !outfitHundler.IsNaked;
        if (canDisguise)
        {
            CurrentInteraction = InteractionType.Disguise;
            return;
        }

        //敵が死んでいて裸なら何もしない
        if (ai.IsDead())
        {
            CurrentInteraction = InteractionType.Locked;
            return;
        }

        //生きていて視界に入ってない ならばキルできる
        if (!characterDetector.IsInvision)
        {
            CurrentInteraction = InteractionType.Kill;
            return;
        }

        //生きていて視界に入っている ならば何もしない
        CurrentInteraction = InteractionType.Locked;
        return;
    }

    public void Interact(InteractionType interactionType)
    {
        humanController.Interacted(interactionType);
    }
    
    public InteractionType GetInteractionType()
    {
        return CurrentInteraction;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public OutfitData GetDisguiseOutfit()
    {
        return outfitHundler.CurrentOutfit;
    }
}