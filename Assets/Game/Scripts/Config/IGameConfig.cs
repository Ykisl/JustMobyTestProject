
using Game.CubeGame.Models;
using System.Collections.Generic;

namespace Game.Config
{
    public interface IGameConfig
    {
        IReadOnlyCollection<CubeModel> AvalibleCubes { get; }
    }
}
