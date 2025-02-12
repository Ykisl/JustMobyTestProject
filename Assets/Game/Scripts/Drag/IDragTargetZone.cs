using UnityEngine;

namespace Game.Drag
{
    public interface IDragTargetZone
    {
        RectTransform TargetDragTransform { get; }

        bool TryPutDraggable(IDraggable draggable, Vector2 localPosition);
    }
}
