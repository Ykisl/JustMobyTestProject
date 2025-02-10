using Game.CubeGame.System;
using Zenject;

namespace Game.Context
{
    public class GameLoader : IInitializable
    {
        private ICubeGameSystem _cubeGameSystem;

        [Inject]
        private void Consturct(
            ICubeGameSystem cubeGameSystem
            )
        {
            _cubeGameSystem = cubeGameSystem;
        }

        public virtual void Initialize()
        {
            _cubeGameSystem.Initialize();
        }
    }
}
