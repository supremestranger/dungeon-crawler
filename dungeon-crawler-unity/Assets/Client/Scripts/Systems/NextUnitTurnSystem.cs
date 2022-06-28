using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client {
    sealed class NextUnitTurnSystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<NextUnitEvent>> _nextUnitEvents = Idents.Worlds.Events;
        readonly EcsFilterInject<Inc<Unit>, Exc<TurnFinished>> _activeUnits = default;
        readonly EcsFilterInject<Inc<TurnFinished>> _finishedUnits = default;

        readonly EcsCustomInject<RoundService> _rs = default;

        public void Run (EcsSystems systems) {
            foreach (var entity in _nextUnitEvents.Value) {
                var (newEntity, ok) = FindNewUnit ();
                
                if (ok) {
                    // successfully found.
                    _rs.Value.ActiveUnit = newEntity;
                } else {
                    // reset "Finished" flag because we change active side
                    var found = false;
                    ClearFinishedFlag ();
                    while (!found) {
                        var newSide = ((int) _rs.Value.ActiveSide + 1) % _rs.Value.StateMax;
                        _rs.Value.ActiveSide = (Side) newSide;
                        var (anotherNewEntity, foundNewUnit) = FindNewUnit ();
                        found = foundNewUnit;
                        _rs.Value.ActiveUnit = anotherNewEntity;
                    }
                }
            }
        }

        void ClearFinishedFlag () {
            foreach (var entity in _finishedUnits.Value) {
                _finishedUnits.Pools.Inc1.Del (entity);
            }
        }

        (int, bool) FindNewUnit () {
            var found = -1;
            var min = int.MaxValue;
            foreach (var entity in _activeUnits.Value) {
                ref var unit = ref _activeUnits.Pools.Inc1.Get (entity);

                if (unit.Initiative < min && unit.Side == _rs.Value.ActiveSide) {
                    found = entity;
                }
            }

            return (found, found >= 0);
        }
    }
}