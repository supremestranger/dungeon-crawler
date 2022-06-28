using UnityEditor;
using UnityEngine;

namespace Client {
#if UNITY_EDITOR
    [ExecuteAlways]
    [SelectionBase]
#endif
    sealed class CellView : MonoBehaviour {
        public Transform Transform;
        public float Size = 3f;

        void Awake () {
            Transform = transform;
        }
        
#if UNITY_EDITOR
        void OnDrawGizmos () {
            var selected = Selection.Contains (gameObject);
            Gizmos.color = selected ? Color.green : Color.cyan;
            var yAdd = selected ? 0.02f : 0f;
            var curPos = Transform.localPosition;
            var leftDown = curPos - Vector3.right * Size / 2 - Vector3.forward * Size / 2 + Vector3.up * yAdd;
            var leftUp = curPos - Vector3.right * Size / 2 + Vector3.forward * Size / 2 + Vector3.up * yAdd;
            var rightDown = curPos + Vector3.right * Size / 2 - Vector3.forward * Size / 2 + Vector3.up * yAdd;
            var rightUp = curPos + Vector3.right * Size / 2 + Vector3.forward * Size / 2 + Vector3.up * yAdd;
            Gizmos.DrawLine (leftDown, leftUp);
            Gizmos.DrawLine (leftUp, rightUp);
            Gizmos.DrawLine (rightUp, rightDown);
            Gizmos.DrawLine (rightDown, leftDown);
            Gizmos.DrawSphere (curPos, 0.1f);
        }
#endif
    }
}