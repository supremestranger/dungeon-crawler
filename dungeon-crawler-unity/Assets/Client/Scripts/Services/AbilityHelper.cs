using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;

namespace Client {
    sealed class AbilityHelper {
        internal delegate int ValidationCheck (in Unit unit, int damage);

        readonly Dictionary<int, Action<EcsWorld, int>> _addComponentCallbacks;
        readonly Dictionary<int, ValidationCheck> _validationCallbacks;

        readonly GridService _gs;

        readonly EcsPool<Cell> _cellPool;

        public AbilityHelper (GridService gs, EcsPool<Cell> cellPool) {
            _addComponentCallbacks = new Dictionary<int, Action<EcsWorld, int>> {
                { 0, AddComponent<LightAttack> },
                { 1, AddComponent<HeavyAttack> },
                { 2, AddComponent<PowerShot>}
            };

            _validationCallbacks = new Dictionary<int, ValidationCheck> {
                { 0, LightAttackValidate },
                { 1, HeavyAttackValidate },
                { 2, PowerShotValidate }
            };

            _gs = gs;
            _cellPool = cellPool;
        }

        void AddComponent<T> (EcsWorld world, int entity) where T : struct {
            world.GetPool<T> ().Add (entity);
        }

        public Action<EcsWorld, int> GetAddComponentCallback (int abilityIdx) {
            return _addComponentCallbacks.TryGetValue (abilityIdx, out Action<EcsWorld, int> cb) ? cb : null;
        }

        public ValidationCheck GetValidateCallback (int abilityIdx) {
            return _validationCallbacks.TryGetValue (abilityIdx, out ValidationCheck cb) ? cb : null;
        }

        int LightAttackValidate (in Unit unit, int damage) {
            var coords = unit.CellCoords;
            var pos3d = Quaternion.Euler (0f, 90f * (int) unit.Direction, 0f) * Vector3.forward;
            var add = new Int2 (Mathf.RoundToInt (pos3d.x), Mathf.RoundToInt (pos3d.z));
            var frontCell = coords + add;
            var (cellEntity, ok) = _gs.GetCell (frontCell);

            if (ok) {
                ref var cell = ref _cellPool.Get (cellEntity);
                if (cell.Entity != -1) {
                    return damage;
                }
            }

            return 0;
        }

        int HeavyAttackValidate (in Unit unit, int damage) {
            var coords = unit.CellCoords;
            var pos3d = Quaternion.Euler (0f, 90f * (int) unit.Direction, 0f) * Vector3.forward;
            var add = new Int2 (Mathf.RoundToInt (pos3d.x), Mathf.RoundToInt (pos3d.z));

            var frontCell = coords + add;
            var (cellEntity, ok) = _gs.GetCell (frontCell);

            if (ok) {
                ref var cell = ref _cellPool.Get (cellEntity);
                if (cell.Entity != -1) {
                    return damage;
                }
            }

            return 0;
        }

        int PowerShotValidate (in Unit unit, int damage) {
            var coords = unit.CellCoords;
            var pos3d = Quaternion.Euler (0f, 90f * (int) unit.Direction, 0f) * Vector3.forward;
            var add = new Int2 (Mathf.RoundToInt (pos3d.x), Mathf.RoundToInt (pos3d.z));
            
            var frontCell = coords + add;
            var (cellEntity, ok) = _gs.GetCell (frontCell);
            if (ok) {
                ref var cell = ref _cellPool.Get (cellEntity);
                if (cell.Entity != -1) {
                    return 0;
                }
            }
            
            frontCell = coords + add * 2;
            var (anotherCellEntity, ok2) = _gs.GetCell (frontCell);
            if (ok2) {
                ref var cell = ref _cellPool.Get (anotherCellEntity);
                if (cell.Entity != -1) {
                    return damage;
                }
            }

            return 0;
        }
    }
}