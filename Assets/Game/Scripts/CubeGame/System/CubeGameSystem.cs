using Game.Config;
using Game.CubeGame.Cube;
using Game.CubeGame.Models;
using Game.CubeGame.Palette;
using Game.Drag;
using System;
using System.Collections.Generic;
using Zenject;

namespace Game.CubeGame.System
{
    public class CubeGameSystem : ICubeGameSystem
    {
        protected IGameConfig _gameConfig;
        protected ICubePaletteSystem _cubePaletteSystem;
        protected ICubeSystem _cubeSystem;
        protected IDragSystem _dragSystem;

        [Inject]
        private void Construct(
            IGameConfig gameConfig,
            ICubePaletteSystem cubePaletteSystem,
            ICubeSystem cubeSystem,
            IDragSystem dragSystem
            )
        {
            _gameConfig = gameConfig;
            _cubePaletteSystem = cubePaletteSystem;
            _cubeSystem = cubeSystem;
            _dragSystem = dragSystem;
        }

        public virtual void Initialize()
        {
            var avalibleCubes = new List<CubeModel>(_gameConfig.AvalibleCubes);

            _cubeSystem.Initialize(avalibleCubes);
            _cubePaletteSystem.Initialzie(avalibleCubes);

            _dragSystem.OnDragFreeFinished += HandleDragFreeFinished;
        }

        public virtual void Deinitialize()
        {
            _dragSystem.OnDragFreeFinished -= HandleDragFreeFinished;
        }

        private void HandleDragFreeFinished(IDraggable draggable)
        {
            if(draggable is not CubeController cube)
            {
                return;
            }

            cube.RemoveWithFade();
        }
    }
}
