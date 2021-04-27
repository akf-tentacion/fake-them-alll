using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using AKUtil;
using UniRx;

/// <summary>
/// 移動部分だけの部分クラス
/// </summary>
public partial class HumanController : MonoBehaviour
{
    static readonly float directionThreshold = 0.1f;
    [SerializeField] float maxVelocity = 5f;
    [SerializeField] float curveVelocity = 0.5f;
    [SerializeField] float moveDrug = 10;
    [SerializeField] [Range(0, 1)] float angularSpeed = 1f;

    float prevVelocity;
    Vector3 nextVelocity;

    StateMachine stateMachine = new StateMachine();
    public State Idle = new State("Idle");
    public State Run = new State("Run");
    public State Walk = new State("Walk");

    partial void OnStart()
    {
        bool isConflict = false;
        //遷移の設定と被りのチェックを行う
        isConflict |= Idle > Walk;
        isConflict |= Idle > Run;
        isConflict |= Walk > Run;
        isConflict |= Walk > Idle;
        isConflict |= Run > Idle;
        isConflict |= Run > Walk;

        stateMachine.SetInitialState(Idle);
       //Assert.IsFalse(isConflict, "遷移が重複しています");

        Idle.OnStart.Subscribe(_ => OnStartIdle());
        Walk.OnStart.Subscribe(_ => OnStartWalk());
        Run.OnStart.Subscribe(_ => OnStartRun());
    }

    void OnStartIdle()
    {
        outfitHundler.Animator.CrossFade("Idle", 0.1f, 0, 0);
    }

    void OnStartRun()
    {
        outfitHundler.Animator.CrossFade("Run", 0.1f, 0, 0);
    }

    void OnStartWalk()
    {
        outfitHundler.Animator.CrossFade("Walk", 0.1f, 0, 0);
    }

    void UpdateVelocity()
    {
        if (input.IsLocked || IsDead)
        {
            nextVelocity = Vector3.zero;
            return;
        }
        var direction = input.Direction;
        var magnitude = direction.magnitude;

        //回転
        var newForward = new Vector3(direction.x, 0, direction.z).normalized;
        newForward = Vector3.Lerp(transform.forward, newForward, angularSpeed);
        transform.forward = newForward;

        if (magnitude <= 0.02f)
        {
            //指をはなしている時は速度を減衰させる
            prevVelocity = Mathf.Max(0, prevVelocity - moveDrug * Time.deltaTime);
            nextVelocity = transform.forward * prevVelocity;
            if (!stateMachine.NowStateIs(Idle)) stateMachine.PushState(Idle);
            return;
        }
        /*
        //回転の制御
        var angleY = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        angleY -= 90f;
        var newAngle = Quaternion.Euler(0f, -angleY, 0f);
        var rotation = Quaternion.Lerp(transform.rotation, newAngle, angularSpeed);
        var velocity = Mathf.Lerp(0,maxVelocity, direction.magnitude);

        rigidbody.MoveRotation(rotation);
        */
        var velocity = Mathf.Lerp(0, maxVelocity, direction.magnitude);
        velocity = Mathf.Min(prevVelocity + 0.5f, velocity);
        var velocityVector = transform.forward * velocity;
        velocityVector.y = rigidbody.velocity.y;

        nextVelocity = velocityVector;
        prevVelocity = velocity;

        if (magnitude <= 0.8f)
        {
            if (!stateMachine.NowStateIs(Walk)) stateMachine.PushState(Walk);
            return;
        }

        if (!stateMachine.NowStateIs(Run)) stateMachine.PushState(Run);
    }

    void UpdateRigidbody()
    {
        rigidbody.velocity = nextVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVelocity();
        //stateMachine.Update();
    }

    private void FixedUpdate()
    {
        UpdateRigidbody();
        //stateMachine.FixedUpdate();
    }

    private void LateUpdate()
    {
       //stateMachine.LateUpdate();
    }
}
