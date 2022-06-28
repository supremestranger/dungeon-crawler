using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class AiCommandsSystem : IEcsRunSystem {
        readonly EcsPoolInject<Unit> _unitPool = default;
        readonly EcsPoolInject<Cell> _cellPool = default;

        readonly EcsPoolInject<TurnFinished> _turnFinishedPool = default;
        readonly EcsPoolInject<NextUnitEvent> _nextUnitEventPool = Idents.Worlds.Events;
        readonly EcsPoolInject<Animating> _animatingPool = default;
        readonly EcsPoolInject<RotateCommand> _rotateCommandPool = default;
        readonly EcsPoolInject<MoveCommand> _moveCommandPool = default;
        readonly EcsPoolInject<HasAbilities> _hasAbilitiesPool = default;
        readonly EcsPoolInject<Apply> _applyPool = default;
        readonly EcsPoolInject<Ability> _abilityPool = default;

        readonly EcsCustomInject<RoundService> _rs = default;
        readonly EcsCustomInject<GridService> _gs = default;
        readonly EcsCustomInject<AbilityHelper> _ah = default;

        public void Run (EcsSystems systems) {
            if (_animatingPool.Value.Has (_rs.Value.ActiveUnit)) {
                return;
            }

            if (_rs.Value.ActiveSide == Side.Enemy) {
                ref var unit = ref _unitPool.Value.Get (_rs.Value.ActiveUnit);
                var cellCoords = unit.CellCoords;
                var userCellCoords = new Int2 ();
                var userEntity = -1;

                // Checking cells that in radius range.
                for (int i = -unit.Radius; i < unit.Radius + 1; i++) {
                    for (int j = -unit.Radius; j < unit.Radius + 1; j++) {
                        var newCellCoords = new Int2 {
                            X = cellCoords.X + i,
                            Y = cellCoords.Y + j
                        };
                        var (cellToCheck, ok) = _gs.Value.GetCell (newCellCoords);

                        if (ok) {
                            ref var cell = ref _cellPool.Value.Get (cellToCheck);
                            if (cell.Entity != -1) {
                                ref var unitOnCell = ref _unitPool.Value.Get (cell.Entity);

                                if (unitOnCell.Side != Side.Enemy) {
                                    userEntity = cell.Entity;
                                    userCellCoords = newCellCoords;
                                    break;
                                }
                            }
                        }
                    }
                }

                // if user is detected, attack or chase him. if user is not detected, finish turn.
                if (userEntity != -1) {
                    var (ability, ok) = CheckAbilitiesValidation (_rs.Value.ActiveUnit);
                    if (ok) {
                        _applyPool.Value.Add (ability);
                    } else {
                        ChasePlayer (cellCoords, userCellCoords, unit);
                    }
                } else {
                    _turnFinishedPool.Value.Add (_rs.Value.ActiveUnit);
                    _nextUnitEventPool.Value.Add (_nextUnitEventPool.Value.GetWorld ().NewEntity ());
                }
            }
        }

        (int, bool) CheckAbilitiesValidation (int activeUnit) {
            ref var hasAbilities = ref _hasAbilitiesPool.Value.Get (activeUnit);
            ref var unit = ref _unitPool.Value.Get (activeUnit);
            var maxDamage = 0;
            var foundAbility = -1;

            for (int i = 0; i < hasAbilities.AbilitiesCount; i++) {
                ref var ability = ref _abilityPool.Value.Get (hasAbilities.Entities[i]);
                // skip if not enough AP.
                if (unit.ActionPoints < ability.ActionPointsCost) {
                    continue;
                }
                var dmg = _ah.Value.GetValidateCallback (ability.Id).Invoke (unit, ability.Damage);
                if (dmg > maxDamage) {
                    foundAbility = hasAbilities.Entities[i];
                    maxDamage = dmg;
                }
            }

            return (foundAbility, foundAbility != -1);
        }

        void ChasePlayer (Int2 activeUnitCoords, Int2 userCoords, in Unit activeUnit) {
            if (userCoords.X != activeUnitCoords.X) {
                // X coord diff
                var diff = Mathf.Clamp (userCoords.X - activeUnitCoords.X, -1, 1);
                var newCoords = activeUnitCoords + new Int2 { X = diff, Y = 0 };
                var (cell, ok) = _gs.Value.GetCell (newCoords);
                var direction = (Direction) ((int) Direction.South - diff);
                if (ok) {
                    // if can move along x axis
                    // rotate to this cell and move there.
                    if (activeUnit.Direction != direction) {
                        ref var rotate = ref _rotateCommandPool.Value.Add (_rs.Value.ActiveUnit);
                        rotate.Side = (int) direction - (int) activeUnit.Direction;
                    } else {
                        _moveCommandPool.Value.Add (_rs.Value.ActiveUnit);
                    }
                    return;
                }
            }

            if (userCoords.Y != activeUnitCoords.Y) {
                // Y coord diff
                var diff = Mathf.Clamp (userCoords.Y - activeUnitCoords.Y, -1, 1);
                var newCoords = activeUnitCoords + new Int2 { X = 0, Y = diff };
                var (cell, ok) = _gs.Value.GetCell (newCoords);
                var direction = (Direction) ((int) Direction.East - diff);
                if (ok) {
                    // if can move along y axis
                    // rotate to this cell and move there.
                    if (activeUnit.Direction != direction) {
                        ref var rotate = ref _rotateCommandPool.Value.Add (_rs.Value.ActiveUnit);
                        rotate.Side = (int) direction - (int) activeUnit.Direction;
                    } else {
                        _moveCommandPool.Value.Add (_rs.Value.ActiveUnit);
                    }
                }
            }
        }
    }
}