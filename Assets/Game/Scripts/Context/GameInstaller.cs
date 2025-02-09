using Game.Config;
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

            RegisterEntryPoint();
        }

        private void RegisterEntryPoint()
        {
            Container.BindInterfacesAndSelfTo<GameLoader>().AsSingle().NonLazy();
        }
    }
}
