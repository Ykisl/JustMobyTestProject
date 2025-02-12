
using Game.Pointer;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

        public event Action<IDraggable> OnDragFreeFinished;

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
            _pointerSystem.OnPointerPressed += HandlePointerPressed;
        }

        public virtual void Dispose()
        {
            _pointerSystem.OnPointerMove -= HandlePointerMove;
            _pointerSystem.OnPointerReleased -= HandlePointerReleased;
            _pointerSystem.OnPointerPressed -= HandlePointerPressed;

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

        protected virtual void TakeDraggable()
        {
            if (!IsDragAvalible())
            {
                return;
            }

            var pointerInfo = _pointerSystem.PointerInfo;
            var position = pointerInfo.CurrentPosition;

            if(!TryGetDragTarget<IDragStartZone>(position, out var startDragTarget))
            {
                return;
            }

            var targetTransform = startDragTarget.TargetDragTransform;
            if(targetTransform == null)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetTransform, position, null, out var localPoint);
            if (!startDragTarget.TryStartDrag(out var draggable, localPoint) || draggable == null || !draggable.IsDragAvalible)
            {
                return;
            }

            RectTransformUtility.ScreenPointToWorldPointInRectangle(_dragRect, position, null, out var mousePoint);

            var offset = draggable.DraggableTransform.position - mousePoint;
            TryStartDrag(draggable, offset);
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

            _currentDraggable.DraggableTransform.position = point;
            var targetScreenPoint = RectTransformUtility.WorldToScreenPoint(null, point);

            if(TryGetDragTarget<IDragTargetZone>(targetScreenPoint, out var dropTarget))
            {
                var targetTransform = dropTarget.TargetDragTransform;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(targetTransform, targetScreenPoint, null, out var localPoint);
                _currentDraggable.DraggableTransform.SetParent(targetTransform);

                if(dropTarget.TryPutDraggable(_currentDraggable, localPoint))
                {
                    ClearDraggable();
                    return;
                }
            }

            _currentDraggable.DraggableTransform.SetParent(_draggableOriginalParent);
            var draggable = _currentDraggable;

            ClearDraggable();
            OnDragFreeFinished?.Invoke(draggable);
        }

        protected virtual void ClearDraggable()
        {
            _currentDraggable = null;
            _draggableOffset = Vector2.zero;
            _draggableOriginalParent = null;
        }

        protected bool TryGetDragTarget<T>(Vector2 position, out T targetZone) where T : class
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                targetZone = default;
                return false;
            }

            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = position;

            var raycastResults = new List<RaycastResult>();
            eventSystem.RaycastAll(eventData, raycastResults);

            foreach ( var raycastResult in raycastResults)
            {
                var gameObjeect = raycastResult.gameObject;
                if(gameObjeect.TryGetComponent<T>(out targetZone))
                {
                    return true;
                }
            }

            targetZone = null;
            return false;
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

        protected virtual void HandlePointerPressed(PointerData data)
        {
            TakeDraggable();
        }
    }
}
