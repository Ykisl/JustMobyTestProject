
using UnityEngine;

namespace Game.Drag
{
    public interface IDragStartZone
    {
        RectTransform TargetDragTransform { get; }

        bool TryStartDrag(out IDraggable draggable, Vector2 localPosition);
    }
}
