using UnityEngine;

namespace Client {
    sealed class SceneData : MonoBehaviour {
        public CellView[] Cells;
#if UNITY_EDITOR
        [ContextMenu ("Find Cells")]
        void FindCells () {
            Cells = FindObjectsOfType<CellView> ();
            Debug.Log ($"Successfully found {Cells.Length} cells!");
        }
#endif
    }
}