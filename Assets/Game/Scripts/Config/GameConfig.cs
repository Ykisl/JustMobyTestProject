using Game.CubeGame.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "[CubeGame] Data/GameConfig")]
    public class GameConfig : ScriptableObject, IGameConfig
    {
        [SerializeField] private List<CubeModel> _avalibleCubes;

        public IReadOnlyCollection<CubeModel> AvalibleCubes => _avalibleCubes;
    }
}
