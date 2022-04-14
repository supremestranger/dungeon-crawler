using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class TimeSystem : IEcsRunSystem {
        readonly EcsCustomInject<TimeService> _ts = default;

        public void Run (EcsSystems systems) {
            _ts.Value.Time = Time.time;
            _ts.Value.UnscaledTime = Time.unscaledTime;
            _ts.Value.DeltaTime = Time.deltaTime;
            _ts.Value.UnscaledDeltaTime = Time.unscaledDeltaTime;
        }
    }
}