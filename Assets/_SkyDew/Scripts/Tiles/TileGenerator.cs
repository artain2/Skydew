using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SkyDew.Tiles
{
    public class TileGenerator : MonoBehaviour
    {
        // Props
        [SerializeField, FoldoutGroup("Params")]
        private Tile prefab;

        [SerializeField, FoldoutGroup("Params")]
        private Transform root;

        [SerializeField, FoldoutGroup("Params")]
        private float tileSize = 1f;

        [SerializeField, FoldoutGroup("Params")]
        private List<TileConfig> allConfigs = new List<TileConfig>();


        // Container
        [SerializeField, DisableInEditorMode, FoldoutGroup("Container")]
        private List<Tile> mapContainer;

        [SerializeField, DisableInEditorMode, FoldoutGroup("Container")]
        private TileConfig[] configContainer;

        [SerializeField, DisableInEditorMode, FoldoutGroup("Container")]
        private Vector2Int mapSize;


        [Button]
        public void Generate(int x, int y)
        {
            mapSize = new Vector2Int(x, y);
            configContainer = new TileConfig[x * y];
            var pos = Vector2.zero;
            for (var xi = 0; xi < x; xi++)
            {
                for (var yi = 0; yi < y; yi++)
                {
                    var inst = Instantiate(prefab, root);
                    inst.transform.localPosition = pos;
                    mapContainer.Add(inst);
                    pos.y += tileSize;
                }

                pos.x += tileSize;
                pos.y = 0;
            }
        }

        [Button]
        public void Clear()
        {
            for (var i = root.childCount - 1; i >= 0; i--)
                DestroyImmediate(root.GetChild(i).gameObject);
            mapContainer.Clear();
            mapSize = Vector2Int.zero;
        }

        public bool TryGetPointByPosition(Vector2 position, out Vector2Int point)
        {
            var rootPos = (Vector2) root.transform.position;
            // var half = tileSize / 2;
            // var maxX = rootPos.x + mapSize.x * tileSize - half;
            // var maxY = rootPos.y + mapSize.y * tileSize - half;
            // var rect = Rect.MinMaxRect(rootPos.x - half, rootPos.y - half, maxX, maxY);
            // if (rect.Contains(position))
            //     return false;
            position -= rootPos;
            var x = Mathf.RoundToInt(position.x / tileSize);
            var y = Mathf.RoundToInt(position.y / tileSize);
            point = new Vector2Int(x, y);

            return IsInRange(x, y);
        }

        public void SetTileInfo(int x, int y, TileConfig config)
        {
            if (!IsInRange(x, y))
                return;

            var index = GetIndexByPoint(x, y);
            configContainer[index] = config;
            mapContainer[index].SetColor(config.tileColor);
        }

        public TilesData GetData(string locationName)
        {
            var data = new TilesData();
            data.locationName = locationName;
            data.size = mapSize;
            Dictionary<string, List<Vector2Int>> dict = new Dictionary<string, List<Vector2Int>>();
            for (var i = 0; i < configContainer.Length; i++)
            {
                if (configContainer[i] == null)
                    continue;

                var tileType = configContainer[i].groundType;
                var point = GetPointByIndex(i);
                if (!dict.ContainsKey(tileType))
                    dict.Add(tileType, new List<Vector2Int>());

                dict[tileType].Add(point);
            }

            var positionsData = dict.Select(x => new TilesData.TilePositions()
            {
                tileType = x.Key,
                positions = x.Value
            }).ToList();
            data.tileTypeData = positionsData;
            return data;
        }

        public void SetData(TilesData data)
        {
            Clear();
            Generate(data.size.x, data.size.y);

            foreach (var typeData in data.tileTypeData)
            {
                var config = allConfigs.FirstOrDefault(x => x.groundType == typeData.tileType);
                foreach (var position in typeData.positions)
                {
                    SetTileInfo(position.x, position.y, config);
                }
            }
        }

        private bool IsInRange(int x, int y)
        {
            return x >= 0 && x < mapSize.x && y >= 0 && y < mapSize.y;
        }

        // public void AddColliders()
        // {
        //     foreach (var tile in mapContainer)
        //     {
        //         if (tile.TryGetComponent<BoxCollider2D>(out var col))
        //             continue;
        //         col = tile.gameObject.AddComponent<BoxCollider2D>();
        //         col.size = Vector2.one * tileSize;
        //     }
        // }
        //
        // public void RemoveColliders()
        // {
        //     foreach (var tile in mapContainer)
        //     {
        //         if (tile.TryGetComponent<BoxCollider2D>(out var col))
        //             continue;
        //         DestroyImmediate(col);
        //     }
        // }

        private int GetIndexByPoint(int x, int y)
        {
            return x * mapSize.y + y;
        }

        private Vector2Int GetPointByIndex(int index)
        {
            var x = index / mapSize.x;
            var y = index % mapSize.y;
            return new Vector2Int(x, y);
        }
    }
}