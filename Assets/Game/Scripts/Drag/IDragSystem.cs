using System;
using UnityEngine;

namespace Game.Drag
{
    public interface IDragSystem
    {
        event Action<IDraggable> OnDragFreeFinished;

        bool TryStartDrag(IDraggable draggable, Vector2 mouseOffset);
        bool IsDragAvalible();
    }
}
