using System;
using UnityEngine;

namespace Game.GamePool
{
    public interface IGamePool : IDisposable
    {
        event Action<IGamePool> OnDispose;

        GameObject Take();

        void Recycle(GameObject gameObject);

        void RecycleAll();

        void ClearAll();
    }
}
