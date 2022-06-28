using UnityEngine;

namespace Client {
    sealed class SceneData : MonoBehaviour {
        public CellView[] Cells;
        public SpawnMarker[] Markers;
        
#if UNITY_EDITOR
        [ContextMenu ("Find Cells")]
        void FindCells () {
            Cells = FindObjectsOfType<CellView> ();
            Debug.Log ($"Successfully found {Cells.Length} cells!");
        }

        [ContextMenu ("Find Markers")]
        void FindMarkers () {
            Markers = FindObjectsOfType<SpawnMarker> ();
            Debug.Log ($"Successfully found {Markers.Length} markers!");
        }
#endif
    }
}