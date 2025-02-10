using Game.Config;
using Game.CubeGame.Palette;
using Game.CubeGame.System;
using Game.Pointer;
using UnityEngine;
using Zenject;

namespace Game.Context
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameConfig _gameConfig;

        public override void InstallBindings()
        {
            Container.Bind<IGameConfig>()
                .To<GameConfig>().FromInstance(_gameConfig);

            Container.BindInterfacesAndSelfTo<BasicPointerSystem>()
                .AsSingle().NonLazy();

            RegisterGameSystems();

            RegisterEntryPoint();
        }

        private void RegisterEntryPoint()
        {
            Container.BindInterfacesAndSelfTo<GameLoader>().AsSingle().NonLazy();
        }

        private void RegisterGameSystems()
        {
            Container.Bind<ICubePaletteSystem>()
                .To<CubePaletteSystem>().AsSingle();

            Container.Bind<ICubeGameSystem>()
                .To<CubeGameSystem>().AsSingle();
        }
    }
}
