using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UniRx;
using AKUtil;

/// <summary>
/// レイにより視界を設定し、指定されたレイヤーのプレイヤーが入ったら通知する。
/// </summary>
public class CharacterDetector : MonoBehaviour
{
    [SerializeField] CircularSectorMeshRenderer circleRenderer;
    [SerializeField] float searchDistance = 0.5f;
    [SerializeField] float angle = 90;
    [SerializeField] float rayIntervalAngle = 1f;  //何度ごとにレイを飛ばすか
    [SerializeField] Layer[] targetLayers;  //レイキャストのターゲット

    Vector3[] rayDirections;
    VisionMaterialData materialData;
    RelationshipHundler relationshipHundler;
    Coroutine colorChangeCoroutine;

    private Subject<Player> DetectSubject = new Subject<Player>();
    private Subject<Unit> LostSubject = new Subject<Unit>();

    public IObservable<Player> OnDetect { get { return DetectSubject; } }
    public IObservable<Unit> OnLost { get { return LostSubject; } }

    public bool IsActive { private set; get; } = true;
    bool isInVision = false;
    internal bool IsInvision { get { return isInVision; } }

    //関係が変わった時に色を変える
    public void SetRelationship(RelationshipHundler relationshipHundler)
    {
        this.relationshipHundler = relationshipHundler;
        ChangeColorImmediately();
        relationshipHundler.OnRelationshipChanged
            .Subscribe(_ => ChangeColor());
    }

    public void Sleep()
    {
        IsActive = false;
        circleRenderer.gameObject.SetActive(false);
    }

    void Awake()
    {
        SetupCircleRenderer();
        SetupRay();
        OnLost.Subscribe(_ => ChangeColorImmediately());
    }

    void ChangeColorImmediately()
    {
        var mat = GetVisionMaterial(relationshipHundler.CurrentRelationshop);
        circleRenderer.SetMaterial(mat);
    }
    void ChangeColor() {
        if (colorChangeCoroutine != null) StopCoroutine(colorChangeCoroutine);
        colorChangeCoroutine = StartCoroutine(ChangeVisionColorCoroutine());
    }

    //追従中にプレイヤーが着替えたりしても、色をそのままにする。
    IEnumerator ChangeVisionColorCoroutine()
    {
        while (isInVision)
        {
            yield return null;
        }

        ChangeColorImmediately();
    }

    private Material GetVisionMaterial(RelationshipType relation)
    {
        bool isHostileAndDetect = relation == RelationshipType.Hostile && IsInvision;
        bool isHostileAndNotDetect = relation == RelationshipType.Hostile && !IsInvision;
        bool isFriendly = relation == RelationshipType.Friendly;

        if (isHostileAndDetect) return materialData.detectedMaterial;

        if (isHostileAndNotDetect) return materialData.dangerMaterial;

        if (isFriendly) return materialData.safeMaterial;

        return materialData.safeMaterial;
    }

    [ContextMenu("SetupVision")]
    void SetupCircleRenderer()
    {
        materialData = DatasetLocator.Instance.VisionMaterialData;

        circleRenderer.degree = angle;
        circleRenderer.radius = searchDistance;
        //扇型の視界がまっすぐに見えるよう調整する
        float baseAngleOffset = 90f - angle / 2;
        circleRenderer.beginOffsetDegree = baseAngleOffset;
    }

    void SetupRay()
    {
        //レイ方向を作成する
        int rayCount = Mathf.FloorToInt(angle / rayIntervalAngle) + 1;

        rayDirections = new Vector3[rayCount];
        float baseAngleOffset = 90f - angle / 2;
        for (int i = 0; i < rayCount; i++)
        {
            var offset = baseAngleOffset + i * rayIntervalAngle;
            var vector = new Vector3(1, 0, 0) * searchDistance;
            vector = Quaternion.Euler(0, offset + 180, 0) * vector;
            rayDirections[i] = vector;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsActive) return;
        var hits = RaycastAll();
        bool detectedNone = hits.Count == 0;

        //視界に何も写っていない
        if (detectedNone) return;

        //何か見つけた
        foreach (var hit in hits)
        {
            var player = hit.collider.gameObject.GetComponent <Player>();
            if (player == null) continue;
            if (player.IsDead) return;
            if (relationshipHundler.CurrentRelationshop == RelationshipType.Friendly) return;
            DetectSubject.OnNext(player);
            isInVision = true;
            ChangeColorImmediately();
            return;
        }

        //視界から消えた
        if (isInVision)
        {
            isInVision = false;
            LostSubject.OnNext(Unit.Default);
            return;
        }

        isInVision = false;
    }

    List<RaycastHit> RaycastAll()
    {
        List<RaycastHit> hits = new List<RaycastHit>();
        foreach(var direction in rayDirections)
        {
            Vector3 rotDir = transform.rotation * direction;
            var ray = new Ray(transform.position, rotDir);
            RaycastHit hit;
            //int mask = targetLayers.GetAllCollisionLayerMask();
            int mask = ~(1 << (int)Layer.Interactive);
            if (Physics.Raycast(transform.position + Vector3.up * 0.3f,rotDir,out hit,searchDistance,mask))hits.Add(hit);
        }
        return hits;
    }

    /// <summary>
    /// ギズモに経路を表示
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var prefPos = transform.position;

        if (rayDirections == null) SetupRay();
        foreach (var direction in rayDirections)
        {
            var rotDir = transform.rotation * direction;
            Gizmos.DrawLine(prefPos, prefPos + rotDir);
        }
    }
}
