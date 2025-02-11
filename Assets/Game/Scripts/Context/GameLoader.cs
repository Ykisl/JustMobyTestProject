using Game.CubeGame.System;
using System;
using Zenject;

namespace Game.Context
{
    public class GameLoader : IInitializable, IDisposable
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

        public void Dispose()
        {
            _cubeGameSystem.Deinitialize();
        }
    }
}
