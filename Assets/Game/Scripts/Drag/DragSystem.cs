
using Game.Pointer;
using System;
using UnityEngine;
using Zenject;

namespace Game.Drag
{
    public class DragSystem : IDragSystem, IInitializable, IDisposable
    {
        protected IPointerSystem _pointerSystem;

        protected IDraggable _currentDraggable;
        protected Vector2 _draggableOffset;
        protected Transform _draggableOriginalParent;

        protected RectTransform _dragRect;

        public event Action OnDragFreeFinished;

        [Inject]
        private void Consturt(IPointerSystem pointerSystem)
        {
            _pointerSystem = pointerSystem;
        }

        public DragSystem(RectTransform dragRect)
        {
            _dragRect = dragRect;
        }

        public virtual void Initialize()
        {
            _currentDraggable = null;
            _draggableOffset = Vector2.zero;

            _pointerSystem.OnPointerMove += HandlePointerMove;
            _pointerSystem.OnPointerReleased += HandlePointerReleased;
        }

        public virtual void Dispose()
        {
            _pointerSystem.OnPointerMove -= HandlePointerMove;
            _pointerSystem.OnPointerReleased -= HandlePointerReleased;

            _currentDraggable = null;
            _draggableOffset = Vector2.zero;
        }

        public virtual bool TryStartDrag(IDraggable draggable, Vector2 mouseOffset)
        {
            if (!IsDragAvalible() || draggable == null || !draggable.IsDragAvalible)
            {
                return false;
            }

            _currentDraggable = draggable; 
            _draggableOffset = mouseOffset;
            _draggableOriginalParent = _currentDraggable.DraggableTransform.parent;

            _currentDraggable.DraggableTransform.SetParent(_dragRect);
            UpdateDraggable();

            return true;
        }

        public virtual bool IsDragAvalible()
        {
            if(_currentDraggable != null)
            {
                return false;
            }

            var pointerInfo = _pointerSystem.PointerInfo;
            return pointerInfo.IsPointerPressed && !pointerInfo.IsPointerReleased;
        }

        protected virtual void UpdateDraggable()
        {
            if (_currentDraggable == null)
            {
                return;
            }

            if (!_currentDraggable.IsDragAvalible)
            {
                ReleaseDraggable();
            }

            var pointerInfo = _pointerSystem.PointerInfo;
            var position = pointerInfo.CurrentPosition;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(_dragRect, position, null, out var point);
            point -= (Vector3)_draggableOffset;

            _currentDraggable.DraggableTransform.position = point;
        }

        protected virtual void ReleaseDraggable()
        {
            if (_currentDraggable == null)
            {
                return;
            }

            var pointerInfo = _pointerSystem.PointerInfo;
            var position = pointerInfo.CurrentPosition;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(_dragRect, position, null, out var point);
            point -= (Vector3)_draggableOffset;

            _currentDraggable.DraggableTransform.SetParent(_draggableOriginalParent);
            _currentDraggable.DraggableTransform.position = point;

            ClearDraggable();
        }

        protected virtual void ClearDraggable()
        {
            _currentDraggable = null;
            _draggableOffset = Vector2.zero;
            _draggableOriginalParent = null;
        }

        protected virtual void HandlePointerMove(PointerData data)
        {
            UpdateDraggable();
        }

        protected virtual void HandlePointerReleased(PointerData data)
        {
            UpdateDraggable();
            ReleaseDraggable();
        }
    }
}
