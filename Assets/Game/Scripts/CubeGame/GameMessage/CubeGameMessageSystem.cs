using Game.CubeGame.Cube;
using Game.CubeGame.System;
using Game.CubeGame.Tower;
using System;
using UnityEngine;
using Zenject;

namespace Game.CubeGame.GameMessage
{
    public class CubeGameMessageSystem : IGameMessageSystem, IInitializable, IDisposable
    {
        protected ICubeGameSystem _cubeGameSystem;
        protected ICubeTowerSystem _towerSystem;

        public event Action<string> OnMessageReceived;

        [Inject]
        private void Consturct(
            ICubeGameSystem cubeGameSystem,
            ICubeTowerSystem towerSystem
            )
        {
            _cubeGameSystem = cubeGameSystem;
            _towerSystem = towerSystem;
        }

        public virtual void Initialize()
        {
            _cubeGameSystem.OnCubeDisappeared += HandleCubeDisappeared;
            _cubeGameSystem.OnCubeDrop += HandleCubeDrop;

            _towerSystem.OnCubeAttached += HandleCubeAttached;
            _towerSystem.OnTowerOwerflow += HandleTowerOwerflow;
        }

        public virtual void Dispose()
        {
            _cubeGameSystem.OnCubeDisappeared -= HandleCubeDisappeared;
            _cubeGameSystem.OnCubeDrop -= HandleCubeDrop;

            _towerSystem.OnCubeAttached -= HandleCubeAttached;
            _towerSystem.OnTowerOwerflow -= HandleTowerOwerflow;
        }

        public virtual void SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            OnMessageReceived?.Invoke(message);
        }

        private void HandleCubeDisappeared(CubeController controller)
        {
            SendMessage("message_cube_disappeared");
        }

        private void HandleCubeDrop(CubeController controller)
        {
            SendMessage("message_cube_dropped");
        }

        private void HandleCubeAttached(TowerCubeData towerCube, Vector2 dropPosition, Vector2 towerPosition)
        {
            SendMessage("message_cube_placed");
        }

        private void HandleTowerOwerflow(TowerCubeData towerCube)
        {
            SendMessage("message_tower_overflow");
        }
    }
}
