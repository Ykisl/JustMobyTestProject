
using Game.CubeGame.Cube;
using System;

namespace Game.CubeGame.System
{
    public interface ICubeGameSystem
    {
        event Action<CubeController> OnCubeDrop;

        void Initialize();
        void Deinitialize();

        void DropCube(CubeController cube);
    }
}
