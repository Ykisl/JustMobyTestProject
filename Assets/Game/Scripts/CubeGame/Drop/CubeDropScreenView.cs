using DG.Tweening;
using Game.CubeGame.Cube;
using Game.CubeGame.System;
using Game.Drag;
using UnityEngine;
using Zenject;

namespace Game.CubeGame.Drop
{
    public class CubeDropScreenView : MonoBehaviour, IDragTargetZone
    {
        [SerializeField] private RectTransform _dropTarget;
        [Space]
        [SerializeField] private Transform _animationStartPoint;
        [SerializeField] private Transform _animationEndPoint;
        [SerializeField] private float _animationDuration = 1f;

        private ICubeGameSystem _cubeGameSystem; 

        public RectTransform TargetDragTransform => _dropTarget;

        [Inject]
        private void Construct(ICubeGameSystem cubeGameSystem)
        {
            _cubeGameSystem = cubeGameSystem;
        }

        public bool TryPutDraggable(IDraggable draggable, Vector2 localPosition)
        {
            if(draggable is not CubeController cube)
            {
                return false;
            }

            DropCube(cube);
            return true;
        }

        protected virtual void DropCube(CubeController cube)
        {
            if(cube == null)
            {
                return;
            }

            if(_animationDuration <= 0f || _animationStartPoint == null || _animationEndPoint == null)
            {
                _cubeGameSystem.DropCube(cube);
                return;
            }

            var startPosition = _animationStartPoint.position;
            var targetPosition = _animationEndPoint.position;

            var tween = cube.transform.DOMove(targetPosition, _animationDuration).From(startPosition)
                .OnComplete(() =>
                {
                    _cubeGameSystem.DropCube(cube);
                });

            tween.Play();
        }
    }
}
