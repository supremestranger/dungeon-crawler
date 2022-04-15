using UnityEditor;
using UnityEngine;

namespace Client {
#if UNITY_EDITOR
    [ExecuteAlways]
    [SelectionBase]
#endif
    sealed class CellView : MonoBehaviour {
        public Transform Transform;
        public float XzStep = 3f;
        public float YStep = 1f;

        void Awake () {
            Transform = transform;
        }

#if UNITY_EDITOR
        void Update () {
            if (!Application.isPlaying && Transform.hasChanged) {
                var newPos = Vector3.zero;
                var curPos = Transform.localPosition;
                newPos.x = Mathf.RoundToInt (curPos.x / XzStep) * XzStep;
                newPos.z = Mathf.RoundToInt (curPos.z / XzStep) * XzStep;
                newPos.y = Mathf.RoundToInt (curPos.y / YStep) * YStep;
                Transform.localPosition = newPos;
            }
        }

        void OnDrawGizmos () {
            var selected = Selection.Contains (gameObject);
            Gizmos.color = selected ? Color.green : Color.cyan;
            var yAdd = selected ? 0.02f : 0f;
            var curPos = Transform.localPosition;
            var leftDown = curPos - Vector3.right * XzStep / 2 - Vector3.forward * XzStep / 2 + Vector3.up * yAdd;
            var leftUp = curPos - Vector3.right * XzStep / 2 + Vector3.forward * XzStep / 2 + Vector3.up * yAdd;
            var rightDown = curPos + Vector3.right * XzStep / 2 - Vector3.forward * XzStep / 2 + Vector3.up * yAdd;
            var rightUp = curPos + Vector3.right * XzStep / 2 + Vector3.forward * XzStep / 2 + Vector3.up * yAdd;
            Gizmos.DrawLine (leftDown, leftUp);
            Gizmos.DrawLine (leftUp, rightUp);
            Gizmos.DrawLine (rightUp, rightDown);
            Gizmos.DrawLine (rightDown, leftDown);
            Gizmos.DrawSphere (curPos, 0.1f);
        }
#endif
    }
}