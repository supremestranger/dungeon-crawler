using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class UnitStartRotatingSystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<Unit, RotateCommand>, Exc<Animated>> _units = default;
        
        readonly EcsPoolInject<Animated> _animatedPool = default;
        readonly EcsPoolInject<Rotating> _rotatingPool = default;
        
        public void Run (EcsSystems systems) {
            foreach (var entity in _units.Value) {
                ref var unit = ref _units.Pools.Inc1.Get (entity);
                ref var rot = ref _units.Pools.Inc2.Get (entity);

                var newDir = (int) unit.Direction + rot.Side;
                if (newDir == -1) {
                    newDir += 4;
                }
                newDir %= 4;
                unit.Direction = (Direction) newDir;
                var actualDir = Quaternion.Euler (0f, 90f * (int) unit.Direction, 0f);
                ref var rotating = ref _rotatingPool.Value.Add (entity);
                _animatedPool.Value.Add (entity);
                rotating.Target = actualDir;
            }
        }
    }
}