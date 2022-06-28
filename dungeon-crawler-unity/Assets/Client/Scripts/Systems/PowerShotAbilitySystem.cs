using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class PowerShotAbilitySystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<PowerShot, Applied>> _powerShotsApplied = default;

        readonly EcsPoolInject<Unit> _unitPool = default;
        readonly EcsPoolInject<Ability> _abilityPool = default;
        readonly EcsPoolInject<DeathEvent> _deathEventPool = default;

        public void Run (EcsSystems systems) {
            foreach (var entity in _powerShotsApplied.Value) {
                Debug.Log ("Power shot applied.");
                ref var ability = ref _abilityPool.Value.Get (entity);
                ref var applied = ref _powerShotsApplied.Pools.Inc2.Get (entity);
                ref var unit = ref _unitPool.Value.Get (applied.Target);

                unit.Health -= ability.Damage;

                if (unit.Health <= 0) {
                    _deathEventPool.Value.Add (applied.Target);
                }
            }
        }
    }
}