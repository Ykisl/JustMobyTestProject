using Game.CubeGame.Cube;
using Game.CubeGame.Models;
using Game.Drag;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.CubeGame.Palette
{
    public class CubePaletteSystem : ICubePaletteSystem
    {
        private ICubeSystem _cubeSystem;
        private IDragSystem _dragSystem;

        private bool _isInitialized = false;
        private List<CubeModel> _paletteCubes;

        public bool IsInitialized => _isInitialized;
        public IReadOnlyList<CubeModel> PaletteCubes => _paletteCubes;

        public event Action OnInitialized;
        public event Action OnPaletteUpdated;

        [Inject]
        private void Consturct(
            ICubeSystem cubeSystem,
            IDragSystem dragSystem
            )
        {
            _cubeSystem = cubeSystem;
            _dragSystem = dragSystem;
        }

        public virtual void Initialzie(IReadOnlyList<CubeModel> paletteCubes)
        {
            _paletteCubes ??= new List<CubeModel>();

            UpdatePalette(paletteCubes);

            _isInitialized = true;
            OnInitialized?.Invoke();
        }

        public virtual void TakeCube(CubeModel cube, Vector2 cubeOffset)
        {
            var newCube = _cubeSystem.CreateCube(cube);
            if(newCube == null)
            {
                return;
            }

            if(!_dragSystem.TryStartDrag(newCube, cubeOffset))
            {
                _cubeSystem.RemoveCube(newCube);
            }
        }

        protected virtual void UpdatePalette(IReadOnlyList<CubeModel> paletteCubes)
        {
            _paletteCubes?.Clear();
            if(paletteCubes != null)
            {
                _paletteCubes?.AddRange(paletteCubes);
            }

            OnPaletteUpdated?.Invoke();
        }
    }
}
