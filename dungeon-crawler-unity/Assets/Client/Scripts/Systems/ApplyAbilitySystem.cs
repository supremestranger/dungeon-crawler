using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using TMPro;
using UnityEngine;

namespace Client {
    sealed class ApplyAbilitySystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<Ability, Apply>> _abilities = default;

        readonly EcsPoolInject<Unit> _unitPool = default;
        readonly EcsPoolInject<Cell> _cellPool = default;
        readonly EcsPoolInject<Applied> _appliedPool = default;
        readonly EcsPoolInject<TurnFinished> _turnFinishedPool = default;
        readonly EcsPoolInject<NextUnitEvent> _nextUnitEventPool = Idents.Worlds.Events;

        readonly EcsCustomInject<GridService> _gs = default;
        readonly EcsCustomInject<RoundService> _rs = default;

        [EcsUguiNamed (Idents.Ui.PlayerAp)]
        readonly TextMeshProUGUI _playerAp = default;
        
        public void Run (EcsSystems systems) {
            foreach (var entity in _abilities.Value) {
                ref var ability = ref _abilities.Pools.Inc1.Get (entity);
                ref var unit = ref _unitPool.Value.Get (ability.OwnerEntity);

                if (unit.Side != _rs.Value.ActiveSide) {
                    continue;
                }
                
                var pos3d = Quaternion.Euler (0f, 90f * (int) unit.Direction, 0f) * Vector3.forward;
                var add = new Int2 (Mathf.RoundToInt (pos3d.x), Mathf.RoundToInt (pos3d.z)) * ability.Distance;

                var cellCoords = unit.CellCoords + add;
                var (cellEntity, ok) = _gs.Value.GetCell (cellCoords);

                if (ok) {
                    ref var cell = ref _cellPool.Value.Get (cellEntity);
                    if (unit.ActionPoints < ability.ActionPointsCost) {
                    }
                    if (cell.Entity != -1 && unit.ActionPoints >= ability.ActionPointsCost) {
                        ref var applied = ref _appliedPool.Value.Add (entity);
                        applied.Target = cell.Entity;
                        unit.ActionPoints -= ability.ActionPointsCost;
                        if (unit.ActionPoints == 0) {
                            unit.ActionPoints = 2;
                            _turnFinishedPool.Value.Add (ability.OwnerEntity);
                            _nextUnitEventPool.Value.Add (_nextUnitEventPool.Value.GetWorld ().NewEntity ());
                        }
                        
                        if (unit.Side == Side.User) {
                            _playerAp.text = unit.ActionPoints.ToString ();
                        }
                    }
                }
            }
        }
    }
}