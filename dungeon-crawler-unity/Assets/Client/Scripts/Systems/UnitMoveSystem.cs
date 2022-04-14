using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class UnitMoveSystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<Unit, Moving>> _movingUnits = default;

        readonly EcsPoolInject<Animated> _animatedPool = default;

        readonly EcsCustomInject<TimeService> _ts = default;

        const float DistanceToStop = 0.1f;

        public void Run (EcsSystems systems) {
            foreach (var entity in _movingUnits.Value) {
                ref var unit = ref _movingUnits.Pools.Inc1.Get (entity);
                ref var move = ref _movingUnits.Pools.Inc2.Get (entity);
                
                unit.Position = Vector3.Lerp (unit.Position, move.Point, unit.MoveSpeed * _ts.Value.DeltaTime);
                unit.Transform.localPosition = unit.Position;

                if ((unit.Position - move.Point).sqrMagnitude <= DistanceToStop) {
                    _animatedPool.Value.Del (entity);
                    _movingUnits.Pools.Inc2.Del (entity);
                }
            }
        }
    }
}