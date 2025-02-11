using Game.CubeGame.Models;
using System.Collections.Generic;

namespace Game.CubeGame.Cube
{
    public interface ICubeSystem
    {
        void Initialize(IReadOnlyList<CubeModel> avalibleCubes);

        CubeController CreateCube(CubeModel cube);
        CubeController CreateCube(string cubeId);
        void RemoveCube(CubeController cube);
    }
}
