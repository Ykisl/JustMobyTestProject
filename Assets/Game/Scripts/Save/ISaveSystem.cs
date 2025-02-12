using System;

namespace Game.Save
{
    public interface ISaveSystem
    {
        event Action OnSaveStateLoaded;

        void LoadState();
        void SaveState();
    }
}