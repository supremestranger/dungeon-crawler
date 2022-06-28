using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using TMPro;
using UnityEngine;

namespace Client {
    sealed class UnitStartMovingSystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<Unit, MoveCommand>, Exc<Animating>> _units = default;

        readonly EcsPoolInject<Animating> _animatingPool = default;
        readonly EcsPoolInject<Moving> _movingPool = default;
        readonly EcsPoolInject<Cell> _cellPool = default;

        readonly EcsCustomInject<GridService> _gs = default;
        readonly EcsCustomInject<RoundService> _rs = default;

        [EcsUguiNamed (Idents.Ui.PlayerAp)]
        readonly TextMeshProUGUI _playerAp = default;
        
        public void Run (EcsSystems systems) {
            foreach (var entity in _units.Value) {
                ref var unit = ref _units.Pools.Inc1.Get (entity);

                if (unit.Side != _rs.Value.ActiveSide) {
                    continue;
                }
                
                ref var cmd = ref _units.Pools.Inc2.Get (entity);

                var step = cmd.Backwards ? -1 : 1;

                var pos3d = Quaternion.Euler (0f, 90f * (int) unit.Direction, 0f) * Vector3.forward;
                var newCellCoords = unit.CellCoords + new Int2 (Mathf.RoundToInt (pos3d.x), Mathf.RoundToInt (pos3d.z)) * step;
                
                var (newCell, ok) = _gs.Value.GetCell (newCellCoords);
                if (ok) {
                    ref var cell = ref _cellPool.Value.Get (newCell);
                    
                    if (cell.Entity != -1) {
                        continue;
                    }

                    var (curCellEntity, _) = _gs.Value.GetCell (unit.CellCoords);
                    ref var curCell = ref _cellPool.Value.Get (curCellEntity);
                    curCell.Entity = -1;
                    
                    cell.Entity = entity;
                    _animatingPool.Value.Add (entity);
                    ref var moving = ref _movingPool.Value.Add (entity);
                    moving.Point = cell.View.Transform.localPosition;
                    unit.CellCoords = newCellCoords;

                    unit.ActionPoints--;
                    if (unit.Side == Side.User) {
                        _playerAp.text = unit.ActionPoints.ToString ();
                    }
                }
            }
        }
    }
}