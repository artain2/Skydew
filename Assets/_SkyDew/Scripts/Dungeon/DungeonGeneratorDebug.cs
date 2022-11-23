using Sirenix.OdinInspector;
using UnityEngine;

namespace _SkyDew.Scripts.Dungeon
{
    public class DungeonGeneratorDebug : MonoBehaviour
    {
        public Vector2Int a;
        public Vector2Int b;
        public RectInt r;
        public RectInt globalBounds;

        [Button]
        public void GetRectNeighbours()
        {
            var points = DungeonGenerator.GeneratorUtil.GetRectNeighbours(r, globalBounds);
            foreach (var p in points)
                Debug.Log(p);
        }

        [Button]
        public void TestMask()
        {
            var mask = GetMask();
            var rect = mask.GetRect(a);
            Debug.Log(rect);
        }

        [Button]
        public void TestMaskRect()
        {
            var mask = GetMask();
            var rect = mask.GetRect(a);
            var points = DungeonGenerator.GeneratorUtil.GetAllPointsInRange(rect, globalBounds);
            foreach (var p in points)
                Debug.Log(p);
        }

        [Button]
        public bool IsLinesIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersect)
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

        private DungeonMask GetMask()
        {
            return new DungeonMask(new short[,] {{1, 1, 1}, {1, 1, 1}});
        }
    }
}