using System;
using UnityEngine;
using Zenject;

namespace Game.Pointer
{
    public class BasicPointerSystem : IPointerSystem, IInitializable, ITickable
    {
        private PointerData _pointerInfo;

        public PointerData PointerInfo => _pointerInfo;

        public event Action<PointerData> OnPointerPressed;
        public event Action<PointerData> OnPointerMove;
        public event Action<PointerData> OnPointerReleased;

        public void Initialize()
        {
            ResetPointer();
        }

        public void Tick()
        {
            var delta = Time.deltaTime;
            UpdatePointer(delta);
        }

        #region Internal

        private void ResetPointer()
        {
            _pointerInfo = default;
        }

        private void UpdatePointer(float delta)
        {
            var pointerPosition = Input.mousePosition;

            if (Input.GetMouseButton(0))
            {
                if (!_pointerInfo.IsPointerPressed || _pointerInfo.IsPointerReleased)
                {
                    StartPointer(pointerPosition);
                    return;
                }

                if (_pointerInfo.IsPointerPressed && !_pointerInfo.IsPointerReleased)
                {
                    UpdatePointer(pointerPosition);
                }

                return;
            }

            if (_pointerInfo.IsPointerPressed && !_pointerInfo.IsPointerReleased)
            {
                EndPointer(pointerPosition);
                return;
            }
        }

        private void StartPointer(Vector2 pointerPosition)
        {
            _pointerInfo.StartPosition = pointerPosition;
            _pointerInfo.CurrentPosition = pointerPosition;
            _pointerInfo.IsPointerPressed = true;
            _pointerInfo.IsPointerReleased = false;

            OnPointerPressed?.Invoke(PointerInfo);
        }

        private void UpdatePointer(Vector2 pointerPosition)
        {
            _pointerInfo.CurrentPosition = pointerPosition;

            OnPointerMove?.Invoke(PointerInfo);
        }

        private void EndPointer(Vector2 pointerPosition)
        {
            _pointerInfo.CurrentPosition = pointerPosition;
            _pointerInfo.IsPointerPressed = true;
            _pointerInfo.IsPointerReleased = true;

            OnPointerReleased?.Invoke(PointerInfo);
        }

        #endregion
    }
}
