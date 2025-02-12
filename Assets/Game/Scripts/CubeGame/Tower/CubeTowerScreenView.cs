using DG.Tweening;
using Game.CubeGame.Cube;
using Game.Drag;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.CubeGame.Tower
{
    public class CubeTowerScreenView : UIBehaviour, IDragTargetZone, IDragStartZone
    {
        [SerializeField] private RectTransform _contentRect;

        private ICubeTowerSystem _cubeTowerSystem;

        protected Rect _viewRect = default;
        protected Sequence _dropSequence;

        public RectTransform TargetDragTransform => _contentRect;

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
            _cubeTowerSystem.OnCubeFall += HandleTowerCubeFall;
            _cubeTowerSystem.OnTowerRebuild += HandleTowerRebuild;
        }

        protected override void OnDisable()
        {
            UpdateViewRect();

            _cubeTowerSystem.OnInitialized -= HanldeSystemInitialized;
            _cubeTowerSystem.OnCubeAttached -= HandleTowerCubeAttached;
            _cubeTowerSystem.OnCubeFall -= HandleTowerCubeFall;
            _cubeTowerSystem.OnTowerRebuild -= HandleTowerRebuild;
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

        public bool TryStartDrag(out IDraggable draggable, Vector2 localPosition)
        {
            draggable = null;

            var towerCubeInPosition = _cubeTowerSystem.GetTowerCubeByPosition(localPosition);
            if (towerCubeInPosition == null)
            {
                return false;
            }

            var cube = towerCubeInPosition.AttachedCube;
            if(cube == null || !cube.IsDragAvalible)
            {
                return false;
            }

            _dropSequence?.Kill(true);
            DOTween.Kill(cube.transform, true);

            _cubeTowerSystem.DetachCube(towerCubeInPosition);
            draggable = cube;
            return true;
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
                DOTween.Kill(cubeTransform, true);

                cubeTransform.SetParent(_contentRect);
                cubeTransform.localPosition = towerCube.Position;
            }
        }

        protected virtual void HanldeSystemInitialized()
        {
            UpdateView();
        }

        protected void CreateFallTween(CubeController cube, Vector2 originalPosition, Vector2 targetPosition)
        {
            var cubeTransform = cube.transform;

            var tween = cubeTransform.DOLocalMove(targetPosition, 0.3f).From(originalPosition);
            tween.Play();
        }

        protected void HandleTowerCubeAttached(TowerCubeData towerCube, Vector2 dropPosition, Vector2 targetPosition)
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

            _dropSequence?.Kill(true);
            _dropSequence = DOTween.Sequence();

            var jumpTween = cubeTransform.DOLocalJump(targetPosition, 10f, 1, 0.2f);
            _dropSequence.Append(jumpTween);
            _dropSequence.Play();
        }

        protected void HandleTowerCubeFall(TowerCubeData towerCube, Vector2 originalPosition, Vector2 targetPosition)
        {
            _dropSequence?.Kill(true);
            _dropSequence = DOTween.Sequence();

            var cube = towerCube.AttachedCube;
            if (cube == null)
            {
                return;
            }

            DOTween.Kill(cube.transform, true);

            var cubeTransform = cube.transform;
            CreateFallTween(cube, originalPosition, targetPosition);
        }

        private void HandleTowerRebuild()
        {
            UpdateView();
        }
    }
}
