using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// 人間の動作を共通化しているクラス
/// </summary>
[RequireComponent(typeof(OutfitHundler))]
[RequireComponent(typeof(Collider))]
public partial class HumanController : MonoBehaviour
{
    [SerializeField] IFTOInput input;
    [SerializeField] Rigidbody rigidbody;

    OutfitHundler outfitHundler;

    public Action OnDead;
    public Action OnDisguise;

    public bool IsDead { private set; get; } = false;

    partial void OnStart();

    void Start()
    {

        input.OnInteract.Subscribe(Interact);
        outfitHundler = GetComponent<OutfitHundler>();
        OnDead += () => IsDead = true;
        OnStart();
    }

    void Interact(IInteractive interactive)
    {
        var type = interactive.GetInteractionType();

        switch (type)
        {
            case InteractionType.Kill:
                Kill(interactive);
                break;
            case InteractionType.Disguise:
                Disguise(interactive);
                break;
            case InteractionType.Use:
                Poison();
                break;
            default:
                break;
        }

        interactive.Interact(type);
    }

    public void Interacted(InteractionType interactionType)
    {
        switch (interactionType)
        {
            case InteractionType.Kill:
                Killed();
                break;
            case InteractionType.Disguise:
                BeNaked();
                break;
            default:
                return;
        }
    }

    Coroutine coroutine;

    void Killed()
    {
        GetComponent<Collider>().isTrigger = true;
        StopCoroutine();
        if (IsDead) return;
        IsDead = true;
        OnDead?.Invoke();
        input.IsLocked = true;
        outfitHundler.Animator.CrossFade("Death", 0, 0, 0);
    }

    void StopCoroutine()
    {
        if (coroutine != null)
        {
            input.IsLocked = false;
            StopCoroutine(coroutine);
        }
    }

    void Kill(IInteractive target)
    {
        Debug.Log("kill");
        transform.LookAt(target.GetPosition());
        StopCoroutine();
        coroutine = StartCoroutine(KillCoroutine());
    }

    IEnumerator KillCoroutine()
    {
        input.IsLocked = true;
        outfitHundler.Animator.CrossFade("Punch", 0, 0, 0);
        yield return new WaitForSeconds(0.48f);
        outfitHundler.Animator.CrossFade("Idle", 0.1f, 0, 0);
        yield return new WaitForSeconds(0.1f);
        input.IsLocked = false;
    }

    void Disguise(IInteractive target)
    {
        StopCoroutine();
        coroutine = StartCoroutine(DisguiseCoroutine(target));
    }

    IEnumerator DisguiseCoroutine(IInteractive target)
    {
        var outfit = target.GetDisguiseOutfit();
        if (outfit == null) yield break;

        input.IsLocked = true;
        outfitHundler.Animator.CrossFade("Disguise", 0, 0, 0);
        yield return new WaitForSeconds(4f/3);
        outfitHundler.SetOutfit(outfit);
        outfitHundler.Animator.CrossFade("Disguise", 0, 0, 0);
        outfitHundler.Animator.CrossFade("Idle", 0.1f, 0, 0);
        yield return new WaitForSeconds(0.1f);
        OnDisguise?.Invoke();
        input.IsLocked = false;

    }

    void Poison()
    {
        input.IsLocked = true;
        outfitHundler.Animator.CrossFade("Stir", 0, 0, 0);
    }


    void BeNaked()
    {
        StopCoroutine();
        coroutine = StartCoroutine(BeNakedCoroutine());
    }

    IEnumerator BeNakedCoroutine()
    {
        yield return new WaitForSeconds(4f / 3);

        outfitHundler.BeNaked();
        outfitHundler.Animator.CrossFade("Idle_Death", 0, 0, 0);
    }

}