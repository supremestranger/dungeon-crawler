using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;

namespace Client {
    sealed class UnitDeathSystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<Unit, DeathEvent>> _deadUnits = default;

        readonly EcsPoolInject<Cell> _cellPool = default;
        readonly EcsPoolInject<Unit> _unitPool = default;

        readonly EcsCustomInject<GridService> _gs = default;
        readonly EcsCustomInject<RoundService> _rs = default;

        [EcsUguiNamed (Idents.Ui.GameOverPopup)]
        readonly GameObject _popup = default;

        public void Run (EcsSystems systems) {
            foreach (var entity in _deadUnits.Value) {
                ref var unit = ref _deadUnits.Pools.Inc1.Get (entity);

                switch (unit.Side) {
                    case Side.Enemy:
                        var (cellEntity, ok) = _gs.Value.GetCell (unit.CellCoords);

                        if (ok) {
                            ref var cell = ref _cellPool.Value.Get (cellEntity);
                            cell.Entity = -1;
                        }

                        unit.View.DieAnim ();
                        break;
                    case Side.User:
                        var (cellEntity2, ok2) = _gs.Value.GetCell (unit.CellCoords);

                        if (ok2) {
                            ref var cell = ref _cellPool.Value.Get (cellEntity2);
                            cell.Entity = -1;
                        }
                        
                        _popup.SetActive (true);
                        break;
                }

                _unitPool.Value.GetWorld ().DelEntity (entity);
            }
        }
    }
}