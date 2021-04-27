using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OutfitLimitedInteraction))]
public class PoisonFinisher : MonoBehaviour
{
    private void Start()
    {
        GetComponent<OutfitLimitedInteraction>().OnInteracted += GameClear;
    }

    void GameClear()
    {
        GameRuleManager.Instance.PlayPoisonFinisher();
    }
}
