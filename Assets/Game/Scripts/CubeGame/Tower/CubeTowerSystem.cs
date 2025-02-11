using Game.CubeGame.Cube;
using Game.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CubeGame.Tower
{
    public class CubeTowerSystem : ICubeTowerSystem
    {  
        private bool _isInitialized;
        protected Rect _towerRect;

        protected List<TowerCubeData> _towerCubes = new List<TowerCubeData>();

        public bool IsInitialzied => _isInitialized;
        public IList<TowerCubeData> ActiveTowerCubes => _towerCubes;

        public event Action OnInitialized;
        public event Action<TowerCubeData, Vector2, Vector2> OnCubeAttached;

        public virtual void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _towerCubes ??= new List<TowerCubeData>();
            _towerCubes.Clear();

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

        public virtual bool TryAttachCube(CubeController cube, Vector2 dropPosition)
        {
            if (!_isInitialized || cube == null)
            {
                return false;
            }

            var towerCube = CreateTowerCube(cube, dropPosition);
            if (!IsCubeInArea(towerCube))
            {
                return false;
            }

            if (!IsTowerEmpty() && IsTowerOverflow())
            {
                return false;
            }

            var cubeAttachPosition = GetCubeAttachPosition(towerCube, dropPosition);
            towerCube.Position = cubeAttachPosition;

            _towerCubes?.Add(towerCube);
            OnCubeAttached?.Invoke(towerCube, dropPosition, cubeAttachPosition);

            return true;
        }

        public virtual bool IsTowerEmpty()
        {
            if (!_isInitialized)
            {
                return true;
            }

            return _towerCubes == null || _towerCubes.Count <= 0;
        }

        public bool IsTowerOverflow()
        {
            if (!_isInitialized)
            {
                return false;
            }

            return GetTowerTopPosition().y > _towerRect.yMax;
        }

        protected TowerCubeData CreateTowerCube(CubeController cube, Vector2 position)
        {
            var cubeRect = cube.CubeRect;
            cubeRect.center = position;

            return new TowerCubeData()
            {
                AttachedCube = cube,
                CubeRect = cubeRect,
            };
        }

        protected virtual bool IsCubeInArea(TowerCubeData cube)
        {
            var cubeRect = cube.CubeRect;
            return cubeRect.IsInsideRect(_towerRect);
        }

        protected virtual Vector2 GetCubeAttachPosition(TowerCubeData cube, Vector2 dropPosition)
        {
            if (IsTowerEmpty())
            {
                return dropPosition;
            }

            var topPosition = GetTowerTopPosition();

            var newCubePosition = topPosition;
            newCubePosition.y += cube.CubeRect.height / 2;
            newCubePosition.x += GetCubeHorizontalOffset(cube);

            return newCubePosition;
        }

        protected virtual float GetCubeHorizontalOffset(TowerCubeData cube)
        {
            var cubeRect = cube.CubeRect;
            var halfSize = cubeRect.width / 2;

            var ofsset = UnityEngine.Random.Range(-halfSize, halfSize);
            return ofsset;
        }

        protected Vector2 GetTowerTopPosition()
        {
            if(IsTowerEmpty())
            {
                return Vector2.zero;
            }

            var lastCube = _towerCubes[_towerCubes.Count - 1];
            var topPosition = lastCube.TopPosition;

            return topPosition;
        }
    }
}
