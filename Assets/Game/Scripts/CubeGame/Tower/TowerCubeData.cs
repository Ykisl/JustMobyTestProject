using Game.CubeGame.Cube;
using UnityEngine;

namespace Game.CubeGame.Tower
{
    public class TowerCubeData
    {
        public Rect CubeRect;
        public CubeController AttachedCube;

        public Vector2 Position
        {
            get => CubeRect.center;
            set => CubeRect.center = value;
        }

        public Vector2 TopPosition
        {
            get => new Vector2(CubeRect.center.x, CubeRect.yMax);
        }

        public Vector2 BottomPosition
        {
            get => new Vector2(CubeRect.center.x, CubeRect.yMin);
        }
    }
}
