using Game.CubeGame.Models;
using Game.GamePool;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.CubeGame.Cube
{
    public class CubeSystem : ICubeSystem
    {
        protected GameObject _rootPoolObject;
        protected CubeController _cubePoolPrefab;

        protected List<CubeModel> _cubeModels;
        protected IGamePool _cubePool;

        public CubeSystem(GameObject poolRootObject, CubeController cubePrefab)
        {
            _rootPoolObject = poolRootObject;
            _cubePoolPrefab = cubePrefab;
        }

        public virtual void Initialize(IReadOnlyList<CubeModel> avalibleCubes)
        {
            InitializeCubePool();
            InitializeAvalibleCubes(avalibleCubes);
        }

        public virtual CubeController CreateCube(CubeModel cubeModel)
        {
            if (_cubePool == null || _cubeModels == null)
            {
                return null;
            }

            if(cubeModel == null || !_cubeModels.Contains(cubeModel))
            {
                return null;
            }

            var newCube = CreateCubeInternal(cubeModel);

            return newCube;
        }

        public virtual CubeController CreateCube(string cubeId)
        {
            if (_cubePool == null || _cubeModels == null)
            {
                return null;
            }

            if(!TryGetCubeModel(cubeId, out var cubeModel))
            {
                return null;
            }

            var newCube = CreateCubeInternal(cubeModel);

            return newCube;

        }

        public virtual void RemoveCube(CubeController cube)
        {
            if(_cubePool == null || cube == null)
            {
                return;
            }

            cube.OnRemove -= HandleCubeRemove;

            cube.SetModel(null);
            _cubePool.Recycle(cube.gameObject);
        }

        #region Internal

        protected virtual CubeController CreateCubeInternal(CubeModel cubeModel)
        {
            var newCubeObject = _cubePool?.Take();
            var newCube = newCubeObject.GetComponent<CubeController>();
            newCube?.SetModel(cubeModel);
            newCube.OnRemove += HandleCubeRemove;

            newCube.gameObject.SetActive(true);

            return newCube;
        }

        private void HandleCubeRemove(CubeController cube)
        {
            RemoveCube(cube);
        }

        protected void InitializeAvalibleCubes(IReadOnlyCollection<CubeModel> cubeModels)
        {
            _cubeModels ??= new List<CubeModel>();
            _cubeModels.Clear();

            if(cubeModels != null)
            {
                _cubeModels.AddRange(cubeModels);
            }
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

        protected void InitializeCubePool()
        {
            if(_cubePool != null)
            {
                return;
            }

            if(_cubePoolPrefab == null)
            {
                Debug.LogError("[CubeSystem] Cube prefab is empty!");
                return;
            }

            if(_rootPoolObject == null)
            {
                Debug.LogWarning($"[CubeSystem] Root pool object is not set, creating..");
                _rootPoolObject = new GameObject("[CubeSystem] CubePoolRoot");
            }

            var newPool = new BaseGamePool(_rootPoolObject);
            newPool.Initialize(_cubePoolPrefab.gameObject);

            _cubePool = newPool;
        }

        #endregion
    }
}

