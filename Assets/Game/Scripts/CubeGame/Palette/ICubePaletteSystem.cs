using Game.CubeGame.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CubeGame.Palette
{
    public interface ICubePaletteSystem
    {
        bool IsInitialized { get; }
        IReadOnlyList<CubeModel> PaletteCubes { get; }

        event Action OnInitialized;
        event Action OnPaletteUpdated;

        void Initialzie(IReadOnlyList<CubeModel> paletteCubes);

        void TakeCube(CubeModel cube, Vector2 cubeOffset);
    }
}
