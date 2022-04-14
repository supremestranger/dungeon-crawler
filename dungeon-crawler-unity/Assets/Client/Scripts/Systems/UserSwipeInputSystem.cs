using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.Scripting;

namespace Client {
    sealed class UserSwipeInputSystem : EcsUguiCallbackSystem {
        readonly EcsFilterInject<Inc<Unit, ControlledByPlayer>> _units = default;

        readonly EcsPoolInject<MoveCommand> _moveCommandPool = default;
        readonly EcsPoolInject<RotateCommand> _rotateCommandPool = default;

        const float MinSwipeMagnitude = 0.2f;

        Vector2 _lastTouchPos = default;

        [Preserve]
        [EcsUguiDownEvent (Idents.Ui.TouchListener, Idents.Worlds.Events)]
        void OnDownTouchListener (in EcsUguiDownEvent e) {
            _lastTouchPos = e.Position;
        }

        [Preserve]
        [EcsUguiUpEvent (Idents.Ui.TouchListener, Idents.Worlds.Events)]
        void OnUpTouchListener (in EcsUguiUpEvent e) {
            var swipe = e.Position - _lastTouchPos;
            var swipeHorizontal = swipe.x / Screen.width;
            var swipeVertical = swipe.y / Screen.height;

            if (Mathf.Abs (swipeVertical) >= MinSwipeMagnitude) {
                foreach (var entity in _units.Value) {
                    ref var moveCmd = ref _moveCommandPool.Value.Add (entity);
                    moveCmd.Backwards = swipeVertical < 0f;
                    break;
                }
            } else if (Mathf.Abs (swipeHorizontal) >= MinSwipeMagnitude) {
                foreach (var entity in _units.Value) {
                    ref var rotCmd = ref _rotateCommandPool.Value.Add (entity);
                    var side = (swipeHorizontal > 0f) ? 1 : 0;
                    rotCmd.Side = side;
                }
            }
        }
    }
}