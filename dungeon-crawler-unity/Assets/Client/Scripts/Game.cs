using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;

namespace Client {
    sealed class Game : MonoBehaviour {
        [SerializeField] SceneData _sceneData;
        [SerializeField] Configuration _configuration;
        [SerializeField] EcsUguiEmitter _uguiEmitter;
        EcsSystems _systems;

        void Start () {
            var world = new EcsWorld ();
            _systems = new EcsSystems (world);
            var ts = new TimeService ();
            var gs = new GridService (_configuration.GridWidth, _configuration.Gridheight);

            _systems
                .Add (new GridInitSystem ())
                .Add (new TimeSystem ())
                .Add (new PlayerInitSystem ())
                .DelHere<MoveCommand> ()
                .DelHere<RotateCommand> ()
                .Add (new UserKeyboardInputSystem ())
                .Add (new UserButtonsInputSystem ())
                .Add (new UserSwipeInputSystem ())
                .Add (new UnitStartMovingSystem ())
                .Add (new UnitMoveSystem ())
                .Add (new UnitStartRotatingSystem ())
                .Add (new UnitRotateSystem ())
                
                .AddWorld (new EcsWorld (), Idents.Worlds.Events)
#if UNITY_EDITOR
                .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
                .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem (Idents.Worlds.Events))
#endif
                .Inject (ts, gs, _sceneData)
                .InjectUgui (_uguiEmitter, Idents.Worlds.Events)
                .Init ();
        }

        void Update () {
            _systems?.Run ();
        }

        void OnDestroy () {
            _systems?.Destroy ();
            _systems?.GetWorld ()?.Destroy ();
            _systems = null;
        }
    }
}