using System;

namespace Game.Pointer
{
    public interface IPointerSystem
    {
        PointerData PointerInfo { get; }

        event Action<PointerData> OnPointerPressed;
        event Action<PointerData> OnPointerMove;
        event Action<PointerData> OnPointerReleased;
    }
}
