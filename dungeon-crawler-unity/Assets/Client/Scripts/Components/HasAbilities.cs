using Leopotam.EcsLite;

namespace Client {
    struct HasAbilities : IEcsAutoReset<HasAbilities> {
        public int[] Entities;
        public int AbilitiesCount;

        public void AutoReset (ref HasAbilities c) {
            c.Entities ??= new int[64];
        }
    }
}