using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField] Player player;

    public Action<IInteractive> OnClick;

    public IInteractive TargetInteraction { get { return player.TargetInteraction; } }

    private void Start()
    {
        player.OnInteractionExit += SetDisable;
        player.OnInteractionUpdate += UpdateButtonView;

        UpdateButtonView(InteractionType.Kill);
        button.interactable = false;
        button.onClick.AddListener(() => OnClick?.Invoke(player.TargetInteraction));
    }

    void SetDisable()
    {
        button.interactable = false;
    }

    void UpdateButtonView(InteractionType type)
    {
        switch (type)
        {
            case InteractionType.Disguise:
                buttonText.text = "変装";
                button.interactable = true;
                break;
            case InteractionType.Kill:
                buttonText.text = "倒す";
                button.interactable = true;
                break;
            case InteractionType.Use:
                buttonText.text = "毒を盛る";
                button.interactable = true;
                break;
            case InteractionType.Locked:
                button.interactable = false;
                return;
            default:
                break;
        }
    }
}
