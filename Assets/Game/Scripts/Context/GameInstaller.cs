using Game.Config;
using Game.CubeGame.Cube;
using Game.CubeGame.Palette;
using Game.CubeGame.System;
using Game.Drag;
using Game.Pointer;
using UnityEngine;
using Zenject;

namespace Game.Context
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameConfig _gameConfig;
        [Space]
        [Header("Cube System")]
        [SerializeField] private GameObject _cubePoolRoot;
        [SerializeField] private CubeController _cubePrefab;
        [Space]
        [Header("Drag system")]
        [SerializeField] private RectTransform _dragRect;

        public override void InstallBindings()
        {
            Container.Bind<IGameConfig>()
                .To<GameConfig>().FromInstance(_gameConfig);

            Container.BindInterfacesAndSelfTo<BasicPointerSystem>()
                .AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<DragSystem>()
                .AsSingle()
                .WithArguments(_dragRect)
                .NonLazy();

            RegisterGameSystems();

            RegisterEntryPoint();
        }

        private void RegisterEntryPoint()
        {
            Container.BindInterfacesAndSelfTo<GameLoader>().AsSingle().NonLazy();
        }

        private void RegisterGameSystems()
        {
            Container.Bind<ICubeSystem>()
                .To<CubeSystem>()
                .FromMethod(() => new CubeSystem(_cubePoolRoot, _cubePrefab))
                .AsSingle();

            Container.Bind<ICubePaletteSystem>()
                .To<CubePaletteSystem>().AsSingle();

            Container.Bind<ICubeGameSystem>()
                .To<CubeGameSystem>().AsSingle();
        }
    }
}
