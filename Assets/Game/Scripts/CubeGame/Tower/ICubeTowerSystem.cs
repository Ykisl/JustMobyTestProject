using Game.CubeGame.Cube;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CubeGame.Tower
{
    public interface ICubeTowerSystem
    {
        bool IsInitialzied { get; }
        IList<TowerCubeData> ActiveTowerCubes { get; }

        event Action OnInitialized;
        event Action<TowerCubeData, Vector2, Vector2> OnCubeAttached;
        event Action<TowerCubeData, Vector2, Vector2> OnCubeFall;
        event Action<TowerCubeData> OnTowerOwerflow;
        event Action OnTowerRebuild;

        void Initialize();
        void SetAvalibleRect(Rect towerRect);
        bool TryAttachCube(CubeController cube, Vector2 position);
        void DetachCube(CubeController cube);
        void DetachCube(TowerCubeData towerCube);
        TowerCubeData GetTowerCubeByPosition(Vector2 position);
        bool IsTowerEmpty();
    }
}
