using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class LightAttackAbilitySystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<LightAttack, Applied>> _lightAttacksApplied = default;

        readonly EcsPoolInject<Unit> _unitPool = default;
        readonly EcsPoolInject<Ability> _abilityPool = default;
        readonly EcsPoolInject<DeathEvent> _deathEventPool = default;

        public void Run (EcsSystems systems) {
            foreach (var entity in _lightAttacksApplied.Value) {
                Debug.Log ("Light attack");
                ref var ability = ref _abilityPool.Value.Get (entity);
                ref var applied = ref _lightAttacksApplied.Pools.Inc2.Get (entity);
                ref var unit = ref _unitPool.Value.Get (applied.Target);

                unit.Health -= ability.Damage;

                if (unit.Health <= 0) {
                    _deathEventPool.Value.Add (applied.Target);
                }
            }
        }
    }
}