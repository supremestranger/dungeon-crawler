using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class HeavyAttackAbilitySystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<HeavyAttack, Applied>> _heavyAttacksApplied = default;

        readonly EcsPoolInject<Unit> _unitPool = default;
        readonly EcsPoolInject<Ability> _abilityPool = default;
        readonly EcsPoolInject<DeathEvent> _deathEventPool = default;

        public void Run (EcsSystems systems) {
            foreach (var entity in _heavyAttacksApplied.Value) {
                Debug.Log ("Heavy attack");
                ref var ability = ref _abilityPool.Value.Get (entity);
                ref var applied = ref _heavyAttacksApplied.Pools.Inc2.Get (entity);
                ref var unit = ref _unitPool.Value.Get (applied.Target);

                unit.Health -= ability.Damage;

                if (unit.Health <= 0) {
                    _deathEventPool.Value.Add (applied.Target);
                }
            }
        }
    }
}