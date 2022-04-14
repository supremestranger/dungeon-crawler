using UnityEngine;

namespace Client {
    sealed class SceneData : MonoBehaviour {
        public CellView[] Cells;

        [ContextMenu ("Find Cells")]
#if UNITY_EDITOR
        void FindCells () {
            Cells = FindObjectsOfType<CellView> ();
            Debug.Log ($"Successfully found {Cells.Length} cells!");
        }
#endif
    }
}