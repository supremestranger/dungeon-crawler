using UnityEngine;

namespace Client {
    [CreateAssetMenu]
    sealed class Configuration : ScriptableObject {
        public int GridWidth;
        public int Gridheight;
    }
}