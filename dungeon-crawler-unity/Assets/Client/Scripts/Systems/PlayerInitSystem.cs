using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class PlayerInitSystem : IEcsInitSystem {
        readonly EcsPoolInject<Unit> _unitPool = default;
        readonly EcsPoolInject<ControlledByPlayer> _controlledByPlayerPool = default;

        public void Init (EcsSystems systems) {
            var playerEntity = _unitPool.Value.GetWorld ().NewEntity ();

            ref var unit = ref _unitPool.Value.Add (playerEntity);
            _controlledByPlayerPool.Value.Add (playerEntity);
            
            var playerPrefab = Resources.Load ("Player");
            var playerGo = (GameObject) Object.Instantiate (playerPrefab, Vector3.zero, Quaternion.identity);

            unit.Direction = 0;
            unit.CellCoords = new Int2 (0, 0);
            unit.Transform = playerGo.transform;
            unit.Position = Vector3.zero;
            unit.Rotation = Quaternion.identity;
            unit.MoveSpeed = 3f;
            unit.RotateSpeed = 10f;
        }
    }
}