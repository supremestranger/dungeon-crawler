using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class UserKeyboardInputSystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<Unit, ControlledByPlayer>> _units = default;

        readonly EcsPoolInject<MoveCommand> _moveCommandPool = default;
        readonly EcsPoolInject<RotateCommand> _rotateCommandPool = default;
        readonly EcsPoolInject<HasAbilities> _hasAbilitiesPool = default;
        readonly EcsPoolInject<Apply> _applyPool = default;
        readonly EcsPoolInject<TurnFinished> _turnFinishedPool = default;
        readonly EcsPoolInject<NextUnitEvent> _nextUnitEventPool = Idents.Worlds.Events;
        readonly EcsPoolInject<Ability> _abilityPool = default;

        readonly EcsCustomInject<AbilityHelper> _ah = default;

        public void Run (EcsSystems systems) {
            foreach (var entity in _units.Value) {
                ref var unit = ref _units.Pools.Inc1.Get (entity);
                var vertInput = Input.GetAxisRaw (Idents.Input.VerticalAxis);
                var horizInput = Input.GetAxisRaw (Idents.Input.HorizontalAxis);
                
                switch (vertInput) {
                    case 1f:
                        _moveCommandPool.Value.Add (entity);
                        break;
                    case -1f:
                        ref var moveCmd = ref _moveCommandPool.Value.Add (entity);
                        moveCmd.Backwards = true;
                        break;
                }

                if (horizInput != 0f) {
                    ref var rotCmd = ref _rotateCommandPool.Value.Add (entity);
                    rotCmd.Side = (int) horizInput;
                }

                if (Input.GetKeyDown (KeyCode.Alpha1)) {
                    TryApplyAbility (0, entity, unit);
                }
                
                if (Input.GetKeyDown (KeyCode.Alpha2)) {
                    TryApplyAbility (1, entity, unit);
                }

                if (Input.GetKeyDown (KeyCode.Alpha3)) {
                    TryApplyAbility (2, entity, unit);
                }

                if (Input.GetKeyDown (KeyCode.Space) && !_turnFinishedPool.Value.Has (entity)) {
                    Debug.Break ();
                    unit.ActionPoints = 2;
                    _turnFinishedPool.Value.Add (entity);
                    _nextUnitEventPool.Value.Add (_nextUnitEventPool.Value.GetWorld ().NewEntity ());
                }
            }
        }

        void TryApplyAbility (int abilityIdx, int entity, in Unit unit) {
            ref var abilities = ref _hasAbilitiesPool.Value.Get (entity);

            var abilityEntity = abilities.Entities[abilityIdx];
            ref var ability = ref _abilityPool.Value.Get (abilityEntity);
            
            var dmg = _ah.Value.GetValidateCallback (ability.Id).Invoke (unit, ability.Damage);
            if (dmg != 0 && unit.ActionPoints >= ability.ActionPointsCost) {
                _applyPool.Value.Add (abilityEntity);
            }
        }
    }
}