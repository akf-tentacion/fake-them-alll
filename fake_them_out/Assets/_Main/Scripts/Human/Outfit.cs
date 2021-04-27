using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit : MonoBehaviour
{
    [SerializeField] Renderer[] renderers;
    [SerializeField] Animator animator;

    public Renderer[] Renderers { get { return renderers; } }
    public Animator Animator { get { return animator; } }
}

public enum OutfitType
{
    Police,
    Bandit,
    Chef,
    Naked,
    Vip,
    Guard,
    Worker
}