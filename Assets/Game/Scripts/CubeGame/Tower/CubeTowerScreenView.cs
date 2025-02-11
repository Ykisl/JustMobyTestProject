using DG.Tweening;
using Game.CubeGame.Cube;
using Game.Drag;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.CubeGame.Tower
{
    public class CubeTowerScreenView : UIBehaviour, IDragTargetZone
    {
        [SerializeField] private RectTransform _contentRect;

        private ICubeTowerSystem _cubeTowerSystem;

        protected Rect _viewRect = default;

        public RectTransform TargetDropTransform => _contentRect;

        [Inject]
        private void Construct(ICubeTowerSystem cubeTowerSystem)
        {
            _cubeTowerSystem = cubeTowerSystem;
        }

        protected override void OnEnable()
        {
            UpdateView();

            _cubeTowerSystem.OnInitialized += HanldeSystemInitialized;
            _cubeTowerSystem.OnCubeAttached += HandleTowerCubeAttached;
        }

        protected override void OnDisable()
        {
            UpdateViewRect();

            _cubeTowerSystem.OnInitialized -= HanldeSystemInitialized;
            _cubeTowerSystem.OnCubeAttached -= HandleTowerCubeAttached;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateViewRect();
        }

        public bool TryPutDraggable(IDraggable draggable, Vector2 localPosition)
        {
            if (draggable is not CubeController cube)
            {
                return false;
            }

            return _cubeTowerSystem.TryAttachCube(cube, localPosition);
        }

        protected virtual void UpdateView()
        {
            UpdateViewRect();
            UpdateTowerCubes();
        }

        protected virtual void UpdateViewRect()
        {
            var contentRect = _contentRect.rect;
            contentRect.center = Vector2.zero;

            _viewRect = contentRect;

            if(_cubeTowerSystem != null && _cubeTowerSystem.IsInitialzied)
            {
                _cubeTowerSystem.SetAvalibleRect(_viewRect);
            }
        }

        protected virtual void UpdateTowerCubes()
        {
            if (!_cubeTowerSystem.IsInitialzied)
            {
                return;
            }

            var towerCubes = _cubeTowerSystem.ActiveTowerCubes;
            if(towerCubes == null)
            {
                return;
            }

            foreach( var towerCube in towerCubes)
            {
                var attachedCube = towerCube.AttachedCube;

                var cubeTransform = attachedCube.transform;
                cubeTransform.SetParent(_contentRect);
                cubeTransform.localPosition = towerCube.Position;
            }
        }

        protected virtual void HanldeSystemInitialized()
        {
            UpdateView();
        }

        private void HandleTowerCubeAttached(TowerCubeData towerCube, Vector2 dropPosition, Vector2 targetPosition)
        {
            var cube = towerCube.AttachedCube;
            if(cube == null)
            {
                return;
            }

            var cubeTransform = cube.transform;
            cubeTransform.SetParent(_contentRect);
            cubeTransform.localPosition = dropPosition;

            var distance = Vector2.Distance(dropPosition, targetPosition);
            if(Mathf.Approximately(distance, 0f))
            {
                cubeTransform.localPosition = targetPosition;
                return;
            }

            var jumpTween = cubeTransform.DOLocalJump(targetPosition, 10f, 1, 0.2f);
            jumpTween.Play();
        }
    }
}
