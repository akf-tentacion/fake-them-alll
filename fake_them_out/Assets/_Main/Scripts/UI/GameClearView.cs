using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameClearView : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Button continueButton;
    [SerializeField] Button exitButton;

    private void Start()
    {
        continueButton.onClick.AddListener(() => SceneManager.LoadScene("Main"));
        exitButton.onClick.AddListener(() => Application.Quit());
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
