using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DefaultExecutionOrder(-1)]
[RequireComponent(typeof(HumanController))]
public class Player : MonoBehaviour, IInteractive
{
    [SerializeField] OutfitData outfitData;
    [SerializeField] OutfitHundler outfitHundler;
    HumanController humanController;

    static readonly float rayRadius = 1.8f;
    public OutfitType CurrentOutfitType { get { return outfitHundler.CurrentOutfit.Type; } }

    public Action<InteractionType> OnInteractionUpdate;
    public Action OnInteractionExit;

    public bool IsDead { get { return humanController.IsDead; } }

    public IInteractive TargetInteraction { private set; get; }

    private void Awake()
    {
        outfitHundler.SetOutfit(outfitData,true);
        humanController = GetComponent<HumanController>();
        humanController.OnDisguise += () =>GameRuleManager.OnPlayerAppearanceChanged?.Invoke(this);
        humanController.OnDead += () => GameRuleManager.Instance.Gameover();
    }

    //球のレイを飛ばして
    private void Update()
    {
        int mask = 1 << (int)Layer.Interactive;
        var cols = Physics.SphereCastAll(transform.position, rayRadius, Vector3.up, 0.01f, mask);

        //ヒットしなかった
        if (cols.Length == 0)
        {
            //干渉先を見失った
            if (TargetInteraction != null)
            {
                TargetInteraction = null;
                OnInteractionExit?.Invoke();

            }
            return;
        }
        //近い順に並べて最初をとる
        var hit = cols.OrderBy(x => (x.transform.position - transform.position).sqrMagnitude).FirstOrDefault();

        var interactive = hit.collider.gameObject.GetComponent<IInteractive>();
        if (interactive == null) return;

        TargetInteraction = interactive;
        OnInteractionUpdate?.Invoke(TargetInteraction.GetInteractionType());

        return;

    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public InteractionType GetInteractionType()
    {
        return InteractionType.Kill;
    }

    public void Interact(InteractionType interactionType)
    {
        humanController.Interacted(interactionType);
    }

    public OutfitData GetDisguiseOutfit()
    {
        return outfitHundler.CurrentOutfit;
    }
}
