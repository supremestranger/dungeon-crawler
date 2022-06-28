using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class AiInitSystem : IEcsInitSystem {
        readonly EcsCustomInject<SceneData> _sceneData = default;
        readonly EcsCustomInject<GridService> _gs = default;
        readonly EcsCustomInject<Configuration> _config = default;
        readonly EcsCustomInject<AbilityHelper> _ah = default;

        readonly EcsPoolInject<HasAbilities> _hasAbilitiesPool = default;
        readonly EcsPoolInject<Ability> _abilityPool = default;

        readonly EcsPoolInject<Cell> _cellPool = default;
        readonly EcsPoolInject<Unit> _unitPool = default;

        public void Init (EcsSystems systems) {
            for (var i = 0; i < _sceneData.Value.Markers.Length; i++) {
                var marker = _sceneData.Value.Markers[i];
                var asset = Resources.Load<UnitView> (Idents.Paths.Units + marker.PrefabName);
                var position = marker.Transform.position;
                var view = Object.Instantiate (asset, position, Quaternion.identity);

                var coords = new Int2 {
                    X = (int) (position.x / _gs.Value.CellSize),
                    Y = (int) (position.z / _gs.Value.CellSize)
                };

                var unitEntity = _unitPool.Value.GetWorld ().NewEntity ();
                ref var unit = ref _unitPool.Value.Add (unitEntity);
                unit.Direction = 0;
                unit.CellCoords = coords;
                var transform = view.transform;
                unit.Transform = transform;
                unit.Position = transform.position;
                unit.Rotation = Quaternion.identity;
                unit.MoveSpeed = 3f;
                unit.RotateSpeed = 10f;
                unit.ActionPoints = 2;
                unit.Initiative = Random.Range (1, 10);
                unit.Health = 3;
                unit.Radius = 2;
                unit.Side = marker.Side;
                unit.View = view;
                
                var (cellEntity, ok) = _gs.Value.GetCell (coords);

                if (ok) {
                    ref var cell = ref _cellPool.Value.Get (cellEntity);

                    cell.Entity = unitEntity;
                }

                CreateAbilities (unitEntity, _unitPool.Value.GetWorld ());
            }
        }
        
        void CreateAbilities (int entity, EcsWorld world) {
            ref var hasAbilities = ref _hasAbilitiesPool.Value.Add (entity);

            for (int i = 0; i < 3; i++) {
                var abilityConfig = _config.Value.AbilitiesConfigs[i];
                var abilityEntity = world.NewEntity ();
                
                ref var ability = ref _abilityPool.Value.Add (abilityEntity);
                ability.ActionPointsCost = abilityConfig.ActionPointsCost;
                ability.OwnerEntity = entity;
                ability.Id = i;
                ability.Damage = abilityConfig.Damage;
                ability.Distance = abilityConfig.Distance;

                _ah.Value.GetAddComponentCallback (i)?.Invoke (world, abilityEntity);
                
                hasAbilities.Entities.Add (abilityEntity);
            }
        }
    }
}