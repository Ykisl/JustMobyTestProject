using Game.CubeGame.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CubeGame.Palette
{
    public class CubePaletteSystem : ICubePaletteSystem
    {
        private bool _isInitialized = false;
        private List<CubeModel> _paletteCubes;

        public bool IsInitialized => _isInitialized;
        public IReadOnlyList<CubeModel> PaletteCubes => _paletteCubes;

        public event Action OnInitialized;
        public event Action OnPaletteUpdated;

        public virtual void Initialzie(IReadOnlyList<CubeModel> paletteCubes)
        {
            _paletteCubes ??= new List<CubeModel>();

            UpdatePalette(paletteCubes);

            _isInitialized = true;
            OnInitialized?.Invoke();
        }

        public virtual void TakeCube(CubeModel cube, Vector2 cubeOffset)
        {

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
