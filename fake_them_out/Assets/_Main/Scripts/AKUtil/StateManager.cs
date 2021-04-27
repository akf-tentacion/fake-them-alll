using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UniRx;

namespace AKUtil
{
    /// <summary>
    /// プッシュダウンステートマシン
    /// 過去の状態を保存しておくことができる。
    /// </summary>
    public class StateMachine
    {
        //変更の監視と値の読み取りだけ公開する
        private ReactiveProperty<State> rpCurrentState = new ReactiveProperty<State>();
        public State CurrentState { private set{ rpCurrentState.Value = value; } get { return rpCurrentState.Value; } }
        public IObservable<State> OnStateChanged { get { return rpCurrentState; } }

        Stack<State> stateStack;

        public StateMachine()
        {
            stateStack = new Stack<State>();
        }

        public void SetInitialState(State next)
        {
            stateStack.Push(next);
            CurrentState = next;
            CurrentState.Enter();
        }

        public void Update()
        {
            CurrentState.Update();
        }

        public void FixedUpdate()
        {
            CurrentState.FixedUdate();
        }

        public void LateUpdate()
        {
            CurrentState.LateUpdate();
        }

        public void PushState(State next)
        {
#if UNITY_EDITOR
            //Debug.Log(CurrentState.Name + " -> " + next.Name);
#endif
            var canShift = CurrentState.CanShiftTo(next);
            Assert.IsTrue(canShift, "その遷移は設定されていません");
            if (!canShift) return;

            CurrentState.Exit();
            stateStack.Push(next);
            CurrentState = next;
            CurrentState.Enter();
        }

        public bool PopState()
        {
            var canPop = stateStack.Count <= 1;
            Assert.IsTrue(canPop, "ステートがスタックされていません");
            if (!canPop) return false;

            stateStack.Pop();
            CurrentState.Exit();
            CurrentState = stateStack.Peek();
            CurrentState.Enter();

            return true;
        }

        public bool NowStateIs(State dest)
        {
            return CurrentState == dest;
        }
    }
}