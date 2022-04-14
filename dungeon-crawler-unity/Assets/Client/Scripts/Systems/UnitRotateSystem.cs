using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class UnitRotateSystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<Unit, Rotating>> _rotatingUnits = default;

        readonly EcsPoolInject<Animated> _animatedPool = default;

        readonly EcsCustomInject<TimeService> _ts = default;

        public void Run (EcsSystems systems) {
            foreach (var entity in _rotatingUnits.Value) {
                ref var unit = ref _rotatingUnits.Pools.Inc1.Get (entity);
                ref var rotate = ref _rotatingUnits.Pools.Inc2.Get (entity);

                unit.Rotation = Quaternion.Lerp (unit.Rotation, rotate.Target, _ts.Value.DeltaTime * unit.RotateSpeed);
                unit.Transform.localRotation = unit.Rotation;

                if ((unit.Rotation.eulerAngles - rotate.Target.eulerAngles).sqrMagnitude <= 0.1f) {
                    _animatedPool.Value.Del (entity);
                    _rotatingUnits.Pools.Inc2.Del (entity);
                }
            }
        }
    }
}