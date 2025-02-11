using UnityEngine;

namespace Game.Drag
{
    public interface IDragTargetZone
    {
        RectTransform TargetDropTransform { get; }

        bool TryPutDraggable(IDraggable draggable, Vector2 localPosition);
    }
}
