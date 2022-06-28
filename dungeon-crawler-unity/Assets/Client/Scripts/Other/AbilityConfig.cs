using UnityEngine;

namespace Client {
    [CreateAssetMenu]
    public class AbilityConfig : ScriptableObject {
        public string Name;
        public int ActionPointsCost;
        public int Damage;
        public int Distance;
    }
}