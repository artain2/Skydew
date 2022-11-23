using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _SkyDew.Scripts.Dungeon
{
    public struct DungeonMask
    {
        public short[,] mask;

        public DungeonMask(short[,] mask)
        {
            this.mask = mask;
        }

        public int GetTotalPrice() => mask.Cast<short>().Sum(x => x); //Aggregate(0, (current, item) => current + item);

        public RectInt GetRect(Vector2Int pnt)
        {
            return new RectInt(pnt.x, pnt.y, mask.GetLength(0), mask.GetLength(1));
        }
    }

    public static class DungeonMasks
    {
        public static readonly Dictionary<string, DungeonMask> Masks = new()
        {
            {
                "1x2", new DungeonMask(new short[,]
                {
                    {1},
                    {1},
                })
            },
            {
                "2x1", new DungeonMask(new short[,]
                {
                    {1, 1},
                })
            },
            {
                "4x2", new DungeonMask(new short[,]
                {
                    {1, 1, 1, 1,},
                    {1, 1, 1, 1},
                })
            },
            {
                "2x4", new DungeonMask(new short[,]
                {
                    {1, 1},
                    {1, 1},
                    {1, 1},
                    {1, 1},
                })
            },
            {
                "3x3", new DungeonMask(new short[,]
                {
                    {1, 1, 1},
                    {1, 1, 1},
                    {1, 1, 1},
                    {1, 1, 1},
                })
            },
            {
                "4x4", new DungeonMask(new short[,]
                {
                    {1, 1, 1, 1},
                    {1, 1, 1, 1},
                    {1, 1, 1, 1},
                    {1, 1, 1, 1},
                })
            },
            {
                "3x5", new DungeonMask(new short[,]
                {
                    {1, 1, 1},
                    {1, 1, 1},
                    {1, 1, 1},
                    {1, 1, 1},
                    {1, 1, 1},
                })
            },
            {
                "5x3", new DungeonMask(new short[,]
                {
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                })
            },
        };
    }
}