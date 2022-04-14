using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class UnitStartMovingSystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<Unit, MoveCommand>, Exc<Animated>> _units = default;

        readonly EcsPoolInject<Animated> _animatedPool = default;
        readonly EcsPoolInject<Moving> _movingPool = default;
        readonly EcsPoolInject<Cell> _cellPool = default;

        readonly EcsCustomInject<GridService> _gs = default;

        public void Run (EcsSystems systems) {
            foreach (var entity in _units.Value) {
                ref var unit = ref _units.Pools.Inc1.Get (entity);
                ref var cmd = ref _units.Pools.Inc2.Get (entity);

                var step = cmd.Backwards ? -1 : 1;

                var pos3d = Quaternion.Euler (0f, 90f * (int) unit.Direction, 0f) * Vector3.forward;
                var newCellCoords = unit.CellCoords + new Int2 (Mathf.RoundToInt (pos3d.x), Mathf.RoundToInt (pos3d.z)) * step;
                
                var (newCell, ok) = _gs.Value.GetCell (newCellCoords);
                if (ok) {
                    ref var cell = ref _cellPool.Value.Get (newCell);
                    _animatedPool.Value.Add (entity);
                    ref var moving = ref _movingPool.Value.Add (entity);
                    moving.Point = cell.View.Transform.localPosition;
                    unit.CellCoords = newCellCoords;
                }
            }
        }
    }
}