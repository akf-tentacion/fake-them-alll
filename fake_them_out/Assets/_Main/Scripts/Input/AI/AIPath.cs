using System;
using UnityEngine;

public class AIPath : MonoBehaviour
{
    [SerializeField] private Point[] path;
    public Point[] Path { get { return path; } }

    [Serializable]
    public class Point
    {
        [SerializeField] Transform point;
        public Vector3 Position { get { return point.position; } }
        public Vector3 Forward { get { return point.forward; } }
        [Header("pointに到達後の待機時間")]
        public float WaitTime;
    }
}
