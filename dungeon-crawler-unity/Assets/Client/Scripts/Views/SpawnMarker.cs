using UnityEngine;

namespace Client {
    sealed class SpawnMarker : MonoBehaviour {
        public Transform Transform;
        public string PrefabName;
        public Side Side;

        public void Awake () {
            Transform = transform;
        }
    }
}