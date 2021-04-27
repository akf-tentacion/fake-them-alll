using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HumanController))]
public class TargetEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<HumanController>().OnDead += () => GameRuleManager.Instance.GameClear(); 
    }

}
