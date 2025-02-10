using Game.CubeGame.Models;
using Game.CubeGame.View;
using Game.GamePool;
using Game.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.CubeGame.Palette
{
    public class CubePaletteView : MonoBehaviour
    {
        [SerializeField] protected DragScrollView _paletteDragBar;
        [Space]
        [SerializeField] protected GameObject _viewItemsPoolRoot;
        [SerializeField] protected CubeItemView _viewItemPrefab;

        protected ICubePaletteSystem _cubePaletteSystem;

        protected IGamePool _viewPool;
        protected Dictionary<CubeItemView, CubeModel> _cubeViewDictionary;
        protected bool _isInitialized;

        [Inject]
        private void Construct(ICubePaletteSystem cubePaletteSystem)
        {
            _cubePaletteSystem = cubePaletteSystem;
        }

        protected virtual void OnEnable()
        {
            GetOrCreateViewPool();

            if (_cubePaletteSystem != null)
            {
                _cubePaletteSystem.OnInitialized += HandleSystemInitialized;
                _cubePaletteSystem.OnPaletteUpdated += HandlePaletteUpdated;
            }

            if(_paletteDragBar != null)
            {
                _paletteDragBar.OnItemDrag += HandleBarItemDrag;
            }

            if (_isInitialized)
            {
                UpdatePaletteCubes();
            }
        }

        protected virtual void OnDisable()
        {
            if (_cubePaletteSystem != null)
            {
                _cubePaletteSystem.OnInitialized -= HandleSystemInitialized;
                _cubePaletteSystem.OnPaletteUpdated -= HandlePaletteUpdated;
            }

            if (_paletteDragBar != null)
            {
                _paletteDragBar.OnItemDrag -= HandleBarItemDrag;
            }
        }

        protected void Start()
        {
            if (_cubePaletteSystem != null && _cubePaletteSystem.IsInitialized)
            {
                Initialize();
            }
        }

        protected virtual void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _cubeViewDictionary ??= new Dictionary<CubeItemView, CubeModel>();
            _cubeViewDictionary.Clear();

            GetOrCreateViewPool();
            UpdatePaletteCubes();
            _isInitialized = true;
        }

        protected void UpdatePaletteCubes()
        {
            var paletteCubes = _cubePaletteSystem?.PaletteCubes;
            paletteCubes ??= new List<CubeModel>();

            SetPaletteCubes(paletteCubes);
        }

        protected virtual void SetPaletteCubes(IReadOnlyList<CubeModel> paletteCubes) 
        {
            ClearPaletteCubes();

            if(paletteCubes == null || paletteCubes.Count <= 0)
            {
                return;
            }

            for( int i = 0; i < paletteCubes.Count; i++ )
            {
                var cubeModel = paletteCubes[i];

                var viewItem = CreateCubeViewItem(cubeModel);
                if(viewItem == null)
                {
                    continue;
                }

                viewItem.transform.SetParent(_paletteDragBar.content);
                viewItem.transform.SetSiblingIndex(i);
                viewItem.gameObject.SetActive(true);

                _cubeViewDictionary.TryAdd(viewItem, cubeModel);
            }
        }
        
        protected virtual void ClearPaletteCubes()
        {
            foreach (var cubeView in _cubeViewDictionary.Keys)
            {
                RecycleCubeItemView(cubeView);
            }

            _cubeViewDictionary.Clear();
        }

        protected CubeItemView CreateCubeViewItem(CubeModel cubeModel)
        {
            if(cubeModel == null)
            {
                return null;
            }

            var pool = GetOrCreateViewPool();

            var viewGameObject = pool.Take();

            var viewItem = viewGameObject.GetComponent<CubeItemView>();
            if(viewItem == null)
            {
                pool.Recycle(viewGameObject);
                return null;
            }

            var viewModel = new CubeItemViewModel
            {
                CubeColor = cubeModel.Color,
            };

            viewItem.SetModel(viewModel);
            return viewItem;
        }

        protected void RecycleCubeItemView(CubeItemView item)
        {
            var pool = GetOrCreateViewPool();

            item.ResetModel();
            pool.Recycle(item.gameObject);
        }

        private IGamePool GetOrCreateViewPool()
        {
            if(_viewPool == null )
            {
                var newPool = new BaseGamePool(_viewItemsPoolRoot);
                newPool.Initialize(_viewItemPrefab.gameObject);
                _viewPool = newPool;
            }

            return _viewPool;
        }

        private void HandleSystemInitialized()
        {
            Initialize();
        }

        private void HandlePaletteUpdated()
        {
            if (!_isInitialized)
            {
                return;
            }

            UpdatePaletteCubes();
        }

        private void HandleBarItemDrag(ScrollItemDragInfo dragInfo)
        {
            if (!_isInitialized)
            {
                return;
            }

            var viewItem = dragInfo.Object?.GetComponent<CubeItemView>();
            if(viewItem == null)
            {
                return;
            }

            if(!_cubeViewDictionary.TryGetValue(viewItem, out var model))
            {
                return;
            }

            _cubePaletteSystem?.TakeCube(model, dragInfo.Offset);
        }
    }
}
