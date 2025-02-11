using UnityEngine;

namespace Game.Extensions
{
    public static class RectExtensions
    {
        public static bool IsInsideRect(this Rect rect, Rect other)
        {
            return rect.xMin >= other.xMin && rect.xMax <= other.xMax && rect.yMin >= other.yMin && rect.yMax <= other.yMax;
        }
    }
}
