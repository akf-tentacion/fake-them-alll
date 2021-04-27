using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using AKUtil;

public class GameRuleManager : SingletonMonoBehaviour<GameRuleManager>
{
    [SerializeField] Player player;
    [SerializeField] GameClearView gameClearView;
    [SerializeField] GameClearView gameoverView;

    internal Player Player { get { return player; } }

    public static Action<Player> OnPlayerAppearanceChanged;

    bool isClear = false;

    public void GameClear()
    {
        isClear = true;
        StartCoroutine(GameClearCoroutine());
    }

    IEnumerator GameClearCoroutine()
    {
        yield return new WaitForSeconds(1);
        gameClearView.Show();

    }

    public void PlayPoisonFinisher()
    {
        isClear = true;
        StartCoroutine(PlayFinisherCoroutine());
    }

    IEnumerator PlayFinisherCoroutine()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Finisher");
    }

    public void Gameover()
    {
        if (isClear) return;
        StartCoroutine(GameoverCoroutine());
    }

    IEnumerator GameoverCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        gameoverView.Show();
    }



}
