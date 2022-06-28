using UnityEngine;

namespace Client {
#if UNITY_EDITOR
    [ExecuteAlways]
    [SelectionBase]
#endif
    sealed class SnapTransform : MonoBehaviour {
        public Transform Transform;
        public float XzStep = 3f;
        public float YStep = 1f;
        
#if UNITY_EDITOR
        void Awake () {
            Transform = transform;
        }

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
#endif
    }
}