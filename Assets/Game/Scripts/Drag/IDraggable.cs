using UnityEngine;

namespace Game.Drag
{
    public interface IDraggable
    {
        Transform DraggableTransform {  get; }
        bool IsDragAvalible { get; }
    }
}
