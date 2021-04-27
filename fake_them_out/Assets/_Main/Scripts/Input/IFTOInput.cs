using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// 移動方向に関する基底クラス
/// </summary>
public class IFTOInput : MonoBehaviour
{
    protected Subject<IInteractive> interactionSubject = new Subject<IInteractive>();
    public IObservable<IInteractive> OnInteract { get { return interactionSubject; } }

    Vector3 direction;

    bool isLocked = false;
    public bool IsLocked { get { return isLocked; } set { if(value)Direction = Vector3.zero; isLocked = value; } }

    public void Reset()
    {
        direction = Vector3.zero;
    }

    public Vector3 Direction
    {
        get { return direction; }
        protected set
        {
            direction = value;
            if (direction.magnitude > 1f) direction = direction.normalized;
        }
    }
}
