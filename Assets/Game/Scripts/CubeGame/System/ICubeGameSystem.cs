
using Game.CubeGame.Cube;
using System;

namespace Game.CubeGame.System
{
    public interface ICubeGameSystem
    {
        event Action<CubeController> OnCubeDrop;
        event Action<CubeController> OnCubeDisappeared;

        void Initialize();
        void Deinitialize();

        void DropCube(CubeController cube);
    }
}
