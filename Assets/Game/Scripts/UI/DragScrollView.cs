using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI 
{
    public struct ScrollItemDragInfo
    {
        public GameObject Object;
        public Vector2 Offset;
    }
    public class DragScrollView : ScrollRect
    {
        [SerializeField] private Vector2 _scrollNormal;
        [SerializeField] private float _dragNormalAccuracy = 0.6f;

        public event Action<ScrollItemDragInfo> OnItemDrag;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive())
                return;

            var pointerDelta = eventData.pressPosition - eventData.position;
            var deltaNormal = Mathf.Abs(Vector2.Dot(pointerDelta.normalized, _scrollNormal.normalized));

            if(deltaNormal <= _dragNormalAccuracy)
            {
                UpdateBounds();
                StartItemDrag(eventData);

                return;
            }

            base.OnBeginDrag(eventData);
        }

        protected virtual void StartItemDrag(PointerEventData eventData)
        {
            if(content == null)
            {
                return;
            }

            var contentItems = content.GetComponentsInChildren<RectTransform>().ToList();
            contentItems.Remove(content);

            RectTransformUtility.ScreenPointToWorldPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out var pressWorldPosition);

            foreach (var contentItem in contentItems)
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(contentItem, eventData.pressPosition))
                {
                    continue;
                }

                var dragOffset = pressWorldPosition - contentItem.position;

                var dragInfo = new ScrollItemDragInfo
                {
                    Object = contentItem.gameObject,
                    Offset = dragOffset,
                };

                OnItemDrag?.Invoke(dragInfo);
            }
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            var gizmoPosition = (Vector2)transform.position;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(gizmoPosition, gizmoPosition + _scrollNormal * 100f);
        }

#endif

    }
}
