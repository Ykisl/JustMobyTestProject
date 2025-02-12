using Game.CubeGame.Cube;
using Game.Extensions;
using Game.Save;
using Game.Save.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game.CubeGame.Tower
{
    public class CubeTowerSystem : ICubeTowerSystem, ISavable
    {  
        protected ICubeSystem _cubeSystem;

        private bool _isInitialized;
        protected Rect _towerRect;

        protected List<TowerCubeData> _towerCubes = new List<TowerCubeData>();

        public bool IsInitialzied => _isInitialized;
        public IList<TowerCubeData> ActiveTowerCubes => _towerCubes;

        public int SaveDataLoadPriority => 0;

        public event Action OnInitialized;
        public event Action<TowerCubeData, Vector2, Vector2> OnCubeAttached;
        public event Action<TowerCubeData, Vector2, Vector2> OnCubeFall;
        public event Action<TowerCubeData> OnTowerOwerflow;
        public event Action OnSaveRequested;
        public event Action OnTowerRebuild;

        [Inject]
        private void Construct(ICubeSystem cubeSystem)
        {
            _cubeSystem = cubeSystem;
        }

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

        public void OnLoadState(ILoadContext context)
        {
            if (!_isInitialized)
            {
                return;
            }

            foreach(var towerCube in _towerCubes)
            {
                towerCube?.AttachedCube?.Remove();
            }

            _towerCubes.Clear();

            var saveData = context.GetData<CubeGameTowerSaveData>();
            if(saveData == null)
            {
                OnTowerRebuild?.Invoke();
                return;
            }

            foreach(var savedCube in saveData.Cubes)
            {
                var cube = _cubeSystem.CreateCube(savedCube.ModelId);
                if(cube == null)
                {
                    continue;
                }

                var towerCube = CreateTowerCube(cube, savedCube.Position);
                _towerCubes.Add(towerCube);
            }

            OnTowerRebuild?.Invoke();
        }

        public void OnSaveState(ISaveContext context)
        {
            if (!_isInitialized)
            {
                return;
            }

            var saveState = new CubeGameTowerSaveData();
            foreach (var towerCube in _towerCubes)
            {
                var cubeModel = towerCube.AttachedCube.Model;
                var cubeData = new CubeGameTowerSaveDataCube
                {
                    ModelId = cubeModel.Id,
                    Position = towerCube.Position
                };

                saveState.Cubes.Add(cubeData);
            }

            context.SetData(saveState);
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
                OnTowerOwerflow?.Invoke(towerCube);
                towerCube.AttachedCube.RemoveWithFade();
                return true;
            }

            if(!IsAvalibleToAttachCube(towerCube))
            {
                return false;
            }

            var cubeAttachPosition = GetCubeAttachPosition(towerCube, dropPosition);
            towerCube.Position = cubeAttachPosition;

            _towerCubes?.Add(towerCube);
            OnCubeAttached?.Invoke(towerCube, dropPosition, cubeAttachPosition);
            OnSaveRequested?.Invoke();

            return true;
        }

        public void DetachCube(CubeController cube)
        {
            if (!_isInitialized)
            {
                return;
            }

            var towerCube = GetTowerCubeFromAttachedCube(cube);
            DetachCube(towerCube);
        }

        public virtual void DetachCube(TowerCubeData towerCube)
        {
            if (!_isInitialized)
            {
                return;
            }

            if (IsTowerEmpty() || !_towerCubes.Contains(towerCube))
            {
                return;
            }

            var cubeIndex = _towerCubes.IndexOf(towerCube);
            var isRootCube = cubeIndex <= 0;

            var fallRootPosition = towerCube.BottomPosition;
            if (!isRootCube)
            {
                var bottomCube = _towerCubes[cubeIndex - 1];
                fallRootPosition = bottomCube.TopPosition;
            }

            for(int i = cubeIndex+1; i < _towerCubes.Count; i++)
            {
                var cubeAbove = _towerCubes[i];

                var originialPosiion = cubeAbove.Position;
                var halfSize = cubeAbove.CubeRect.size/ 2;

                var xOffset = originialPosiion.x - fallRootPosition.x;

                var targetPosition = fallRootPosition;
                targetPosition.y += halfSize.y;
                targetPosition.x = Mathf.Clamp(targetPosition.x + xOffset, fallRootPosition.x - halfSize.x, fallRootPosition.x + halfSize.x);


                cubeAbove.Position = targetPosition;
                fallRootPosition = cubeAbove.TopPosition;

                OnCubeFall?.Invoke(cubeAbove, originialPosiion, targetPosition);
            }

            _towerCubes.RemoveAt(cubeIndex);
            OnSaveRequested?.Invoke();
        }

        public virtual TowerCubeData GetTowerCubeByPosition(Vector2 position)
        {
            if (!_isInitialized)
            {
                return null;
            }

            if (IsTowerEmpty())
            {
                return null;
            }

            foreach (var towerCube in _towerCubes)
            {
                if (towerCube.CubeRect.Contains(position))
                {
                    return towerCube;
                }
            }

            return null;
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

        protected virtual bool IsAvalibleToAttachCube(TowerCubeData cube)
        {
            if (!_isInitialized)
            {
                return false;
            }

            if (IsTowerEmpty())
            {
                return true;
            }

            var cubeRect = cube.CubeRect;
            foreach(var towerCube in _towerCubes)
            {
                if (cubeRect.Overlaps(towerCube.CubeRect))
                {
                    return true;
                }
            }

            return false;
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

            var newRect = cube.CubeRect;
            newRect.center = newCubePosition;
            newCubePosition.x += GetCubeHorizontalOffset(newRect);

            return newCubePosition;
        }

        protected virtual float GetCubeHorizontalOffset(Rect cubeRect)
        {
            var halfSize = cubeRect.width / 2;

            var leftMaxSize = cubeRect.xMin - _towerRect.xMin;
            leftMaxSize = Mathf.Min(leftMaxSize, halfSize);

            var rightMaxSize = _towerRect.xMax - cubeRect.xMax;
            rightMaxSize = Mathf.Min(rightMaxSize, halfSize);

            var ofsset = UnityEngine.Random.Range(-leftMaxSize, rightMaxSize);
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

        protected TowerCubeData GetTowerCubeFromAttachedCube(CubeController attachedCube)
        {
            if (IsTowerEmpty())
            {
                return null;
            }

            var towerCube = _towerCubes.FirstOrDefault(x => x.AttachedCube == attachedCube);
            return towerCube;
        }
    }
}
