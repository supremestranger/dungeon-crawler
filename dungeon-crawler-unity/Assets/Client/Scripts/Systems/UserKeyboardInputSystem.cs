using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class UserKeyboardInputSystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<Unit, ControlledByPlayer>> _units = default;

        readonly EcsPoolInject<MoveCommand> _moveCommandPool = default;
        readonly EcsPoolInject<RotateCommand> _rotateCommandPool = default;

        public void Run (EcsSystems systems) {
            foreach (var entity in _units.Value) {
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
            }
        }
    }
}