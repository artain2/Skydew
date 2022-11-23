using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkyDew.Tiles
{
    [Serializable]
    public class TilesData
    {
        public string locationName;
        public Vector2Int size;
        public List<TilePositions> tileTypeData = new List<TilePositions>();

        [Serializable]
        public class TilePositions
        {
            public string tileType;
            public List<Vector2Int> positions = new List<Vector2Int>();
        }
    }
}