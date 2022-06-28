using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;

namespace Client {
    sealed class PlayerInitSystem : IEcsInitSystem {
        readonly EcsPoolInject<Unit> _unitPool = default;
        readonly EcsPoolInject<ControlledByPlayer> _controlledByPlayerPool = default;
        readonly EcsPoolInject<HasAbilities> _hasAbilitiesPool = default;
        readonly EcsPoolInject<Ability> _abilityPool = default;

        readonly EcsCustomInject<RoundService> _rs = default;
        readonly EcsCustomInject<Configuration> _config = default;
        readonly EcsCustomInject<AbilityHelper> _ah = default;

        [EcsUguiNamed (Idents.Ui.Abilities)]
        readonly Transform _abilitiesLayoutGroup = default;
        [EcsUguiNamed (Idents.Ui.GameOverPopup)]
        readonly GameObject _popup = default;
        
        public void Init (EcsSystems systems) {
            var world = _unitPool.Value.GetWorld ();
            var playerEntity = world.NewEntity ();

            ref var unit = ref _unitPool.Value.Add (playerEntity);
            _controlledByPlayerPool.Value.Add (playerEntity);
            
            var playerPrefab = Resources.Load<UnitView> ("Player");
            var playerView =  Object.Instantiate (playerPrefab, Vector3.zero, Quaternion.identity);

            _rs.Value.ActiveUnit = playerEntity;
            
            unit.Direction = 0;
            unit.CellCoords = new Int2 (0, 0);
            unit.Transform = playerView.transform;
            unit.Position = Vector3.zero;
            unit.Rotation = Quaternion.identity;
            unit.MoveSpeed = 3f;
            unit.RotateSpeed = 10f;
            unit.ActionPoints = 2;
            unit.Health = 10;
            unit.Initiative = Random.Range (1, 10);
            unit.Side = Side.User;
            unit.View = playerView;

            CreateAbilities (playerEntity, world);
            _popup.SetActive (false);
        }

        void CreateAbilities (int playerEntity, EcsWorld world) {
            var abilityAsset = Resources.Load<AbilityView> ("Ability");
            ref var hasAbilities = ref _hasAbilitiesPool.Value.Add (playerEntity);
            
            for (int i = 0; i < 3; i++) {
                var abilityConfig = _config.Value.AbilitiesConfigs[i];
                var abilityEntity = world.NewEntity ();
                
                ref var ability = ref _abilityPool.Value.Add (abilityEntity);
                ability.ActionPointsCost = abilityConfig.ActionPointsCost;
                ability.OwnerEntity = playerEntity;
                ability.Id = i;
                ability.Damage = abilityConfig.Damage;
                ability.Distance = abilityConfig.Distance;

                var abilityView = Object.Instantiate (abilityAsset, _abilitiesLayoutGroup);
                abilityView.Name.text = $"{abilityConfig.Name}\nCost: {abilityConfig.ActionPointsCost}";
                abilityView.AbilityIdx = i;
                abilityView.KeyIdx.text = (i + 1).ToString();
                _ah.Value.GetAddComponentCallback (i)?.Invoke (world, abilityEntity);
                
                hasAbilities.Entities.Add (abilityEntity);
            }
        }
    }
}