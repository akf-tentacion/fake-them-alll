using System;
using System.Collections.Generic;
using UniRx;

namespace AKUtil
{
    public class State
    {
        public string Name { get; private set; }
        private Subject<Unit> StartSubject = new Subject<Unit>();
        private Subject<Unit> EndSubject = new Subject<Unit>();
        private Subject<Unit> UpdateSubject = new Subject<Unit>();
        private Subject<Unit> FixedUpdateSubject = new Subject<Unit>();
        private Subject<Unit> LateUpdateSubject = new Subject<Unit>();
        public IObservable<Unit> OnStart { get { return StartSubject; } }
        public IObservable<Unit> OnEnd { get { return EndSubject; } }
        public IObservable<Unit> OnUpdate { get { return UpdateSubject; } }
        public IObservable<Unit> OnFixedUpdate { get { return FixedUpdateSubject; } }
        public IObservable<Unit> OnLateUpdate { get { return LateUpdateSubject; } }
        HashSet<State> nextStates = new HashSet<State>();
        public State(string name)
        {
            Name = name;
        }
        public static bool operator >(State current, State next)
        {
            return !current.nextStates.Add(next);
        }
        public static bool operator <(State next, State prev)
        {
            return prev > next;
        }
        public bool CanShiftTo(State next)
        {
            return (this.nextStates.Contains(next));
        }

        public void Update()
        {
            UpdateSubject.OnNext(Unit.Default);
        }
        public void FixedUdate()
        {
            FixedUpdateSubject.OnNext(Unit.Default);
        }
        public void LateUpdate()
        {
            LateUpdateSubject.OnNext(Unit.Default);
        }

        public void Enter()
        {
            StartSubject.OnNext(Unit.Default);
        }

        public void Exit()
        {
            EndSubject.OnNext(Unit.Default);
        }
    }
}