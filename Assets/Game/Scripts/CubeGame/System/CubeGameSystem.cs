using Game.Config;
using Game.CubeGame.Cube;
using Game.CubeGame.Models;
using Game.CubeGame.Palette;
using System.Collections.Generic;
using Zenject;

namespace Game.CubeGame.System
{
    public class CubeGameSystem : ICubeGameSystem
    {
        protected IGameConfig _gameConfig;
        protected ICubePaletteSystem _cubePaletteSystem;
        protected ICubeSystem _cubeSystem;

        [Inject]
        private void Construct(
            IGameConfig gameConfig,
            ICubePaletteSystem cubePaletteSystem,
            ICubeSystem cubeSystem
            )
        {
            _gameConfig = gameConfig;
            _cubePaletteSystem = cubePaletteSystem;
            _cubeSystem = cubeSystem;
        }

        public virtual void Initialize()
        {
            var avalibleCubes = new List<CubeModel>(_gameConfig.AvalibleCubes);

            _cubeSystem.Initialize(avalibleCubes);
            _cubePaletteSystem.Initialzie(avalibleCubes);
        }
    }
}
