using Game.CubeGame.Cube;
using Game.Drag;
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
            UpdateViewRect();
        }

        protected override void OnDisable()
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

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateViewRect();
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
    }
}
