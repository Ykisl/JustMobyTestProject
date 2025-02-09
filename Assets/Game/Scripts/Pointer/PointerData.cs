using UnityEngine;

namespace Game.Pointer
{
    public struct PointerData
    {
        public bool IsPointerPressed;
        public bool IsPointerReleased;

        public Vector2 StartPosition;
        public Vector2 CurrentPosition;

        public Vector2 Delta
        {
            get => StartPosition - CurrentPosition;
        }
    }
}
