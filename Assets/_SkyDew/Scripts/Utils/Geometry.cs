using UnityEngine;

namespace Utils
{
    public static class Geometry
    {
        
        public static bool IsLinesIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersect)
        {
            var denominator = (b2.y - b1.y) * (a1.x - a2.x) - (b2.x - b1.x) * (a1.y - a2.y);
            intersect = default;
            if (denominator == 0)
                return false;

            var numA = (b2.x - a2.x) * (b2.y - b1.y) - (b2.x - b1.x) * (b2.y - a2.y);
            var numB = (a1.x - a2.x) * (b2.y - a2.y) - (b2.x - a2.x) * (a1.y - a2.y);
            var uA = numA / denominator;
            var uB = numB / denominator;
            var hasIntersect = uA is >= 0 and <= 1 && uB is >= 0 and <= 1;
            if (!hasIntersect)
                return false;
            var x = a1.x * uA + a2.x * (1 - uA);
            var y = a1.y * uA + a2.y * (1 - uA);
            intersect = new Vector2(x, y);
            return true;
        }
    }
}