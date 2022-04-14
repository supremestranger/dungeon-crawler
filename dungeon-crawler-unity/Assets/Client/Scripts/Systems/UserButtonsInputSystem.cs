using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine.Scripting;

namespace Client {
    sealed class UserButtonsInputSystem : EcsUguiCallbackSystem {
        readonly EcsFilterInject<Inc<Unit, ControlledByPlayer>> _units = default;

        readonly EcsPoolInject<MoveCommand> _moveCommandPool = default;
        readonly EcsPoolInject<RotateCommand> _rotateCommandPool = default;

        [Preserve]
        [EcsUguiClickEvent (Idents.Ui.Forward, Idents.Worlds.Events)]
        void OnClickForward (in EcsUguiClickEvent e) {
            foreach (var entity in _units.Value) {
                _moveCommandPool.Value.Add (entity);
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent (Idents.Ui.Back, Idents.Worlds.Events)]
        void OnClickBack (in EcsUguiClickEvent e) {
            foreach (var entity in _units.Value) {
                ref var moveCmd = ref _moveCommandPool.Value.Add (entity);
                moveCmd.Backwards = true;
                break;
            }
        }

        [Preserve]
        [EcsUguiClickEvent (Idents.Ui.Left, Idents.Worlds.Events)]
        void OnClickLeft (in EcsUguiClickEvent e) {
            foreach (var entity in _units.Value) {
                ref var rotCmd = ref _rotateCommandPool.Value.Add (entity);
                rotCmd.Side = -1;
            }
        }
        
        [Preserve]
        [EcsUguiClickEvent (Idents.Ui.Right, Idents.Worlds.Events)]
        void OnClickRight (in EcsUguiClickEvent e) {
            foreach (var entity in _units.Value) {
                ref var rotCmd = ref _rotateCommandPool.Value.Add (entity);
                rotCmd.Side = 1;
            }
        }
    }
}