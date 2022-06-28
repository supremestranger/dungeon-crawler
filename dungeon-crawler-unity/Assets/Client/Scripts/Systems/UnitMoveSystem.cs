using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using TMPro;
using UnityEngine;

namespace Client {
    sealed class UnitMoveSystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<Unit, Moving>> _movingUnits = default;

        readonly EcsPoolInject<Animating> _animatingPool = default;
        readonly EcsPoolInject<TurnFinished> _turnFinishedPool = default;
        readonly EcsPoolInject<NextUnitEvent> _nextUnitEventPool = Idents.Worlds.Events;

        readonly EcsCustomInject<TimeService> _ts = default;
        
        [EcsUguiNamed (Idents.Ui.PlayerAp)]
        readonly TextMeshProUGUI _playerAp = default;

        const float DistanceToStop = 0.001f;

        public void Run (EcsSystems systems) {
            foreach (var entity in _movingUnits.Value) {
                ref var unit = ref _movingUnits.Pools.Inc1.Get (entity);
                ref var move = ref _movingUnits.Pools.Inc2.Get (entity);
                
                unit.Position = Vector3.Lerp (unit.Position, move.Point, unit.MoveSpeed * _ts.Value.DeltaTime);

                if ((unit.Position - move.Point).sqrMagnitude <= DistanceToStop) {
                    unit.Position = move.Point;
                    _animatingPool.Value.Del (entity);
                    _movingUnits.Pools.Inc2.Del (entity);
                    if (unit.ActionPoints == 0) {
                        unit.ActionPoints = 2;
                        if (unit.Side == Side.User) {
                            _playerAp.text = unit.ActionPoints.ToString ();
                        }
                        _turnFinishedPool.Value.Add (entity);
                        _nextUnitEventPool.Value.Add (_nextUnitEventPool.Value.GetWorld ().NewEntity ());
                    }
                }
                
                unit.Transform.localPosition = unit.Position;
            }
        }
    }
}