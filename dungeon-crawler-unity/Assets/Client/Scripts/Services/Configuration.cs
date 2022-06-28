using UnityEngine;

namespace Client {
    [CreateAssetMenu]
    sealed class Configuration : ScriptableObject {
        public int GridWidth;
        public int GridHeight;
        public LayerMask UnitLayerMask;
        public AbilityConfig[] AbilitiesConfigs;
    }
}