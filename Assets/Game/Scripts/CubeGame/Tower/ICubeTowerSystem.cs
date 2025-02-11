using Game.CubeGame.Cube;
using System;
using UnityEngine;

namespace Game.CubeGame.Tower
{
    public interface ICubeTowerSystem
    {
        bool IsInitialzied { get; }

        event Action OnInitialized;

        void Initialize();

        void SetAvalibleRect(Rect towerRect);

        bool TryAttachCube(CubeController cube, Vector2 position);
    }
}
