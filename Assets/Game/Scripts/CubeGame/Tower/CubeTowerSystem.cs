using Game.CubeGame.Cube;
using Game.Extensions;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.CubeGame.Tower
{
    public class CubeTowerSystem : ICubeTowerSystem
    {  
        private bool _isInitialized;
        protected Rect _towerRect;

        public bool IsInitialzied => _isInitialized;

        public event Action OnInitialized;

        public virtual void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _towerRect = default;

            _isInitialized = true;
            OnInitialized?.Invoke();
        }

        public virtual void SetAvalibleRect(Rect towerRect)
        {
            if (!_isInitialized)
            {
                return;
            }

            _towerRect = towerRect;
        }

        public virtual bool TryAttachCube(CubeController cube, Vector2 position)
        {
            if (!_isInitialized)
            {
                return false;
            }

            if (!IsCubeInArea(cube, position))
            {
                return false;
            }

            return true;
        }

        protected virtual bool IsCubeInArea(CubeController cube, Vector2 position) 
        {
            var cubeRect = cube.CubeRect;
            cubeRect.center = position;

            return cubeRect.IsInsideRect(_towerRect);
        }
    }
}
