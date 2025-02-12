using Game.CubeGame.System;
using Game.Save;
using System;
using Zenject;

namespace Game.Context
{
    public class GameLoader : IInitializable, IDisposable
    {
        private ICubeGameSystem _cubeGameSystem;
        private ISaveSystem _saveSystem;

        [Inject]
        private void Consturct(
            ICubeGameSystem cubeGameSystem,
            ISaveSystem saveSystem
            )
        {
            _cubeGameSystem = cubeGameSystem;
            _saveSystem = saveSystem;
        }

        public virtual void Initialize()
        {
            _cubeGameSystem.Initialize();
            _saveSystem.LoadState();
        }

        public void Dispose()
        {
            _cubeGameSystem.Deinitialize();
        }
    }
}
