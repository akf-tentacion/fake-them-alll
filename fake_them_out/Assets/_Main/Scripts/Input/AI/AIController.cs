using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UniRx;
using AKUtil;

/// <summary>
/// ステートベースの敵AI
/// パトロールして、敵を見つけたら追いかけて、見失ったらまた持ち場に戻る。
/// Navmeshによる経路探索を行い、その経路を利用して移動方向のinputを行う。
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(OutfitHundler))]
[RequireComponent(typeof(CharacterDetector))]
[RequireComponent(typeof(HumanController))]
public class AIController : IFTOInput
{
    [SerializeField] AIPath aiPath;
    [SerializeField] EnemyData enemyData;

    OutfitHundler outfitHundler;
    CharacterDetector characterDetector;
    HumanController humanController;
    NavMeshAgent navMeshAgent;

    private StateMachine stateMachine = new StateMachine();
    private RelationshipHundler relationshipHundler;
    private IInteractive chaseTarget;

    private static readonly float arriveDiffThreshold = 0.1f;
    private static readonly float killDiffThreshold = 1.05f;

    public State Patrol = new State("Patrol");
    public State Chase = new State("Chase");
    public State Dead = new State("Dead");

    public bool IsDead()
    {
        return IsCurrentState(Dead);
    }

    //購読だけ公開する
    private Subject<RelationshipType> relationshipChangeSubject = new Subject<RelationshipType>();
    public IObservable<RelationshipType> OnRelationshipChanged { get { return relationshipChangeSubject; } }

    public IObservable<State> OnStateChanged { get { return stateMachine.OnStateChanged; } }

    private bool isHostile = false;

    public bool IsCurrentState(State target)
    {
        return target == stateMachine.CurrentState;
    }

    private void Awake()
    {
        Setup();
    }

    void Setup()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        humanController = GetComponent<HumanController>();
        outfitHundler = GetComponent<OutfitHundler>();
        characterDetector = GetComponent<CharacterDetector>();

        bool isConflict = false;

        //遷移の設定と被りのチェックを行う
        isConflict |= Patrol > Chase;
        isConflict |= Chase > Patrol;
        isConflict |= Patrol > Dead;
        isConflict |= Chase > Dead;

        Assert.IsFalse(isConflict, "遷移が重複しています");

        relationshipHundler = new RelationshipHundler(enemyData);
        outfitHundler.Initialize(enemyData.OutfitData, relationshipHundler);

    }
    [ContextMenu("Setoutfit")]
    void Setoutfit()
    {
        var oh = GetComponent<OutfitHundler>();
        oh.SetOutfit(enemyData.OutfitData);
    }

    private void Start()
    {
        Patrol.OnStart.Subscribe(_ => OnStartPatrol());
        Patrol.OnUpdate.Subscribe(_ => UpdatePatrol());
        Chase.OnStart.Subscribe(_ => OnStartChase());
        Chase.OnUpdate.Subscribe(_ => OnUpdateChase());
        Chase.OnEnd.Subscribe(_ => OnEndChase());
        Dead.OnStart.Subscribe(_ => OnStartDead());

        stateMachine.SetInitialState(Patrol);

        characterDetector.SetRelationship(relationshipHundler);
        characterDetector.OnDetect.Subscribe(DetectCallback);
        characterDetector.OnLost.Subscribe(_ => LostCollback());

        humanController.OnDead += () => stateMachine.PushState(Dead);
    }

    int pathIndex = 0;
    int cornerIndex = 0;
    AIPath.Point currentTargetPoint;
    Vector3 currentCorner;
    Vector3 moveStartPoint;
    Vector3 positionBuffer;
    Vector3 lookDirection;
    float interval = 0;
    float timer = 0;

    //巡回するために最初の経路を設定する
    void OnStartPatrol()
    {
        Debug.Log("pat");
        moveStartPoint = transform.position;

        //pathが存在しない
        if (aiPath == null || aiPath.Path == null) return;
        currentTargetPoint = aiPath.Path[pathIndex];
        lookDirection = currentTargetPoint.Forward;
        navMeshAgent.SetDestination(currentTargetPoint.Position);
        moveStartPoint = transform.position;
        timer = 10000;
        Direction = Vector3.zero;
    }

    //設定されたパスに沿って巡回する
    void UpdatePatrol()
    {
        navMeshAgent.SetDestination(currentTargetPoint.Position);
        //その場で待機する
        if (timer < interval)
        {
            Direction = lookDirection * 0.001f;
            timer += Time.deltaTime;
            return;
        }

        //pathが存在しない
        if (aiPath == null || aiPath.Path == null) return;

        var navPath = navMeshAgent.path.corners;
        if (navPath.Length <= 0) return;

        //経路が存在しないか、十分近づいたら到着とみなす
        bool hasArrived = navPath.Length <= 1 || (transform.position - currentTargetPoint.Position).sqrMagnitude <= arriveDiffThreshold;
        if (hasArrived)
        {
            Direction = Vector3.zero;
            interval = aiPath.Path[pathIndex].WaitTime;
            lookDirection = currentTargetPoint.Forward;
            pathIndex++;
            pathIndex %= aiPath.Path.Length;

            moveStartPoint = transform.position;
            currentTargetPoint = aiPath.Path[pathIndex];
            navMeshAgent.SetDestination(currentTargetPoint.Position);
            //待機
            timer = 0;

            return;
        }

        //移動の入力を行う
        positionBuffer = transform.position;
        var posDiff = (navPath[1] - positionBuffer).normalized / 2;
        Direction = posDiff;

    }

    void LostCollback()
    {
        if (IsCurrentState(Patrol)) return;
        stateMachine.PushState(Patrol);
    }

    void DetectCallback(IInteractive target)
    {
        if (IsCurrentState(Chase)) return;
        chaseTarget = target;
        stateMachine.PushState(Chase);
    }

    void OnStartChase()
    {
        navMeshAgent.SetDestination(chaseTarget.GetPosition());
    }

    void OnUpdateChase()
    {
        navMeshAgent.SetDestination(chaseTarget.GetPosition());

        var navPath = navMeshAgent.path.corners;
        if (navPath.Length <= 0) return;
        //経路が存在しないか、十分近づいたら到着とみなす
        bool hasArrived = navPath.Length <= 1 || (transform.position - chaseTarget.GetPosition()).sqrMagnitude <= killDiffThreshold;
        if (hasArrived)
        {
            interactionSubject.OnNext(chaseTarget);
            stateMachine.PushState(Patrol);
            Direction = Vector3.zero;
            Debug.Log("Player killed");

            return;
        }

        //移動の入力を行う
        positionBuffer = transform.position;
        var posDiff = (navPath[1] - positionBuffer).normalized;
        Direction = posDiff;

    }

    void OnEndChase()
    {
        Direction = Vector3.zero;
    }
    void OnStartDead()
    {
        characterDetector.Sleep();
        Direction = Vector3.zero;
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    private void LateUpdate()
    {
        stateMachine.LateUpdate();
    }

    /// <summary>
    /// ギズモに経路を表示
    /// </summary>
    void OnDrawGizmos()
    {
        if (navMeshAgent && navMeshAgent.enabled)
        {
            Gizmos.color = Color.red;
            var prefPos = transform.position;

            foreach (var pos in navMeshAgent.path.corners)
            {
                Gizmos.DrawLine(prefPos, pos);
                prefPos = pos;
            }
        }
    }
}

/// <summary>
/// プレイヤーとの敵対関係の変化
/// </summary>
public class RelationshipHundler
{
    /// <summary>
    /// 値が変更された時にメッセージを流すプロパティ
    /// 値の読み取りと監視のみ公開する
    /// </summary>
    private ReactiveProperty<RelationshipType> rpCurrentRelationship = new ReactiveProperty<RelationshipType>();
    public IObservable<RelationshipType> OnRelationshipChanged { get { return rpCurrentRelationship; } }
    public RelationshipType CurrentRelationshop { get { return rpCurrentRelationship.Value; } }

    private EnemyData enemyData;

    public RelationshipHundler(EnemyData enemyData)
    {
        this.enemyData = enemyData;

        var player = GameRuleManager.Instance.Player;
        ChangeRelationship(player);

        //プレイヤーの外見が変わると、敵対関係が変化する
        GameRuleManager.OnPlayerAppearanceChanged += ChangeRelationship;
    }

    private void ChangeRelationship(Player player)
    {
        if (enemyData.Hostiles.Contains(player.CurrentOutfitType))
        {
            ChangeIntoHostile();
            return;
        }
        ChangeIntoFriendly();
    }

    void ChangeIntoHostile()
    {
        rpCurrentRelationship.Value = RelationshipType.Hostile;
    }

    void ChangeIntoFriendly()
    {
        rpCurrentRelationship.Value = RelationshipType.Friendly;
    }

}

public enum RelationshipType
{
    Hostile,
    Friendly
}
