using Game.Config;
using Game.CubeGame.Models;
using Game.CubeGame.Palette;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Game.CubeGame.System
{
    public class CubeGameSystem : ICubeGameSystem
    {
        protected IGameConfig _gameConfig;
        protected ICubePaletteSystem _cubePaletteSystem;

        protected List<CubeModel> _cubeModels;

        [Inject]
        private void Construct(
            IGameConfig gameConfig,
            ICubePaletteSystem cubePaletteSystem
            )
        {
            _gameConfig = gameConfig;
            _cubePaletteSystem = cubePaletteSystem;
        }

        public virtual void Initialize()
        {
            InitializeAvalibleCubes(_gameConfig.AvalibleCubes);

            _cubePaletteSystem.Initialzie(_cubeModels);
        }

        #region Internal

        protected void InitializeAvalibleCubes(IReadOnlyCollection<CubeModel> cubeModels)
        {
            _cubeModels ??= new List<CubeModel>();
            _cubeModels.Clear();

            _cubeModels.AddRange(cubeModels);
        }

        protected bool TryGetCubeModel(string cubeId, out CubeModel cubeModel)
        {
            cubeModel = null;

            if (_cubeModels == null || _cubeModels.Count <= 0) 
            {
                return false;
            }

            cubeModel = _cubeModels.FirstOrDefault(x => x.Id == cubeId);
            return cubeModel != null;
        }

        #endregion
    }
}
