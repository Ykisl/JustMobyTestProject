
using Game.Save.Context;
using System;

namespace Game.Save
{
    public interface ISavable
    {
        int SaveDataLoadPriority { get; }

        event Action OnSaveRequested;

        void OnLoadState(ILoadContext context);
        void OnSaveState(ISaveContext context);
    }
}
