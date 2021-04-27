using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// インプット
/// </summary>
public class SwipeController : IFTOInput
{
    [SerializeField] InteractButton button;

    static readonly float sensitivity = 0.0062f;

    Vector3 prevPos;
    Vector3 currentPos;

    private void Start()
    {
        button.OnClick += Interact;
        Direction = Vector3.zero;
    }

    void Update ()
    {
        if (IsLocked) return;

        UpdateMousePosition();
    }

    bool isHoldingAnyKey;

    void UpdateMousePosition()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        var space = Input.GetKeyDown(KeyCode.Space);
        var input = new Vector3(x, 0, y);
        if (input.sqrMagnitude >= 0.01f)
        {
            Direction = new Vector3(x, 0, y).normalized;
            isHoldingAnyKey = true;
        }

        if (input.sqrMagnitude < 0.01f && isHoldingAnyKey)
        {
            Direction = Vector3.zero;
            isHoldingAnyKey = false;
        }

        if (space)
        {
            Interact();
        }

        var onDown = Input.GetMouseButtonDown(0);
        var down = Input.GetMouseButton(0);
        var onUp = Input.GetMouseButtonUp(0);

        if (onDown)
        {
            prevPos = currentPos = Input.mousePosition;
            return;
        }

        if (down)
        {
            //prevPos = currentPos;
            currentPos = Input.mousePosition;
            UpdateController();
            return;
        }

        if (onUp)
        {
            prevPos = currentPos;
            Direction = Vector3.zero;
            return;
        }
    }

    void Interact()
    {
        if (button.TargetInteraction == null) return;
        interactionSubject.OnNext(button.TargetInteraction);
    }

    void Interact(IInteractive target)
    {
        if (target == null) return;
        interactionSubject.OnNext(target);
    }

    void UpdateController()
    {
        var delta = currentPos - prevPos;
        delta.z = delta.y;
        delta.y = 0;
        Direction = delta * sensitivity;
    }
}
