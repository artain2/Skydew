using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SkyDew.Tiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _SkyDew.Scripts.Dungeon
{
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField] private Vector2Int size = new (100, 100);
       // [SerializeField] private DungeonTilesInfo lockedTilesInfo;
        [SerializeField] private DungeonTilesInfo freeTilesInfo;
        [SerializeField] private TileGenerator tiles;
        [SerializeField] private TileConfig lockedTileConfig;
        [SerializeField] private TileConfig normalTileConfig;
        [SerializeField] private TileConfig tunnelTileConfig;
       // [SerializeField] private int lockSourcePoints = 4;
        [SerializeField] private int lastSeed = 0;
        [SerializeField] private bool newSeed = true;

        private short[,] _map;
        // private SpaceGenerator _rockGenerator;
        private SpaceGenerator _tunnelGenerator;
        // private bool _rockCompleted = false;
        private bool _tunnelCompleted = false;
        
        [Button]
        public void Prepare()
        {
            if (newSeed)
                lastSeed = Random.Range(0, int.MaxValue);
            // _rockCompleted = false;
            _tunnelCompleted = false;
            Random.InitState(lastSeed);
            tiles.Clear();
            _map = new short[size.x, size.y];
            // _rockGenerator = new SpaceGenerator(lockedTilesInfo, lockSourcePoints, _map, -1);
            // _rockGenerator.Generate();
            _tunnelGenerator = new SpaceGenerator(freeTilesInfo, 1, _map, 1);
            _tunnelGenerator.Generate();
            tiles.Generate(size.x, size.y);
        }

        [Button]
        public void Step()
        {
            if (_tunnelCompleted)
            {
                Debug.Log("Completed");
                return;
            }

            // if (!_rockCompleted)
            // {
            //     _rockCompleted = !_rockGenerator.Step();
            //     if (_rockCompleted)
            //     {
            //     }
            //
            //     RepaintTiles();
            //     return;
            // }

            if (!_tunnelCompleted)
            {
                _tunnelCompleted = !_tunnelGenerator.Step();
                RepaintTiles();
            }
        }

        [Button]
        public void Generate()
        {
            Prepare();
            // while (!_rockCompleted)
            // {
            //     _rockCompleted = !_rockGenerator.Step();
            // }
            _tunnelGenerator.Generate();
            while (!_tunnelCompleted)
            {
                _tunnelCompleted = !_tunnelGenerator.Step();
            }
            RepaintTiles();
        }

        private void RepaintTiles()
        {
            for (var x = 0; x < size.x; x++)
                for (var y = 0; y < size.y; y++)
                    tiles.SetTileInfo(x, y, GetTileConfig(_map[x, y]));
        }

        private TileConfig GetTileConfig(short value)
        {
            return value switch
            {
                1 => tunnelTileConfig,
                -1 => lockedTileConfig,
                _ => normalTileConfig
            };
        }

        private class SpaceGenerator
        {
            private DungeonTilesInfo _tilesInfo;
            private short[,] _map;
            private Vector2Int _size;
            private int _sourcePointsCount;
            private List<Vector2Int> _sourcePoints = new();
            private List<Vector2Int> _pointsToPlace = new();
            private RectInt globalBounds;
            private short _value = -1;
            private int _tilesLeft;


            public SpaceGenerator(DungeonTilesInfo tilesInfo, int sourcePoints, short[,] map, short value)
            {
                _tilesInfo = tilesInfo;
                _tilesLeft = tilesInfo.tilesCount;
                _sourcePointsCount = sourcePoints;
                _map = map;
                _value = value;
                _size = GeneratorUtil.GetTableSize(map);
                globalBounds = new RectInt(0, 0, _size.x, _size.y);
            }

            public void Generate()
            {
                _pointsToPlace.Clear();
                CreateSpawnPoints();
                _pointsToPlace.AddRange(_sourcePoints);
            }

            private short this[Vector2Int p]
            {
                get => _map[p.x, p.y];
                set => _map[p.x, p.y] = value;
            }

            private void CreateSpawnPoints()
            {
                _sourcePoints.Clear();
                
                for (var i = 0; i < _sourcePointsCount; i++)
                {
                    var rndPoint = GetFreePoint();
                    _sourcePoints.Add(rndPoint);
                }

                Vector2Int GetFreePoint()
                {
                    for (int i = 0; i < 500; i++)
                    {
                        var rndPoint = GeneratorUtil.GetRandomPoint(_size);
                        if (this[rndPoint] == 0)
                            return rndPoint;
                    }

                    throw new Exception("Cant find free tile");
                }
            }

            public bool Step()
            {
                var maskKey = Weights.GetWeightObject(_tilesInfo.areas).area;
                var mask = DungeonMasks.Masks[maskKey];
                InsertArea(mask);
                _tilesLeft -= mask.GetTotalPrice();
                return _tilesLeft > 0;
            }

            private void InsertArea(DungeonMask area)
            {
                GeneratorUtil.ShuffleArray(_pointsToPlace);
                Vector2Int insertPoint = default;
                var resultFound = false;
                for (int i = 0; i < _pointsToPlace.Count; i++)
                {
                    if (CanInsertArea(area, _pointsToPlace[i], out insertPoint))
                    {
                        resultFound = true;
                        break;
                    }
                }

                if (!resultFound)
                {
                    Debug.LogError("Cant insert");
                }

                // inserting mask in map (it certainly in good position)
                var maskRect = area.GetRect(insertPoint);
                var maskPoints = GeneratorUtil.GetAllPointsInRange(maskRect, globalBounds);
                foreach (var maskPoint in maskPoints)
                {
                    _map[maskPoint.x, maskPoint.y] = _value;
                }

                // Remove obsolete points to place
                var rectNeighbours = GeneratorUtil.GetRectNeighbours(maskRect, globalBounds);
                foreach (var rNgb in rectNeighbours)
                {
                    if (this[rNgb] != _value)
                    {
                        if (!_pointsToPlace.Contains(rNgb))
                        {
                            _pointsToPlace.Add(rNgb);
                        }
                    }
                }

                // Add new spawn points
                foreach (var maskPnt in maskPoints)
                {
                    // if (_sourcePoints.Contains(maskPnt))
                        _pointsToPlace.Remove(maskPnt);
                    // if (GeneratorUtil.HasNeighbour(maskPnt, _map, n => this[n] == 0))
                    //     _pointsToPlace.Add(maskPnt);
                }
            }

            private bool CanInsertArea(DungeonMask area, Vector2Int point, out Vector2Int insertPoint)
            {
                var goodOffsets = new List<Vector2Int>();
                var maskSize = GeneratorUtil.GetTableSize(area.mask);

                var tryPositionsRect = new RectInt(
                    point.x - maskSize.x + 1,
                    point.y - maskSize.y + 1,
                    maskSize.x,
                    maskSize.y);

                var tryPositions = GeneratorUtil.GetAllPointsInRange(tryPositionsRect, globalBounds);
                foreach (var tryPosition in tryPositions)
                {
                    if (!TryInsertInPoint(tryPosition))
                        continue;
                    goodOffsets.Add(tryPosition);
                }

                bool TryInsertInPoint(Vector2Int tryPoint)
                {
                    for (var x = tryPoint.x; x < tryPoint.x + maskSize.x; x++)
                    {
                        for (var y = tryPoint.y; y < tryPoint.y + maskSize.y; y++)
                        {
                            var pnt = new Vector2Int(x, y);
                            if (!globalBounds.Contains(pnt))
                                return false;
                            if (_map[x, y] != 0)
                                return false;
                        }
                    }

                    return true;
                }

                var any = goodOffsets.Any();
                insertPoint = any ? goodOffsets[Random.Range(0, goodOffsets.Count)] : default;
                return any;
            }
        }

        public static class GeneratorUtil
        {
            private static Vector2Int[] Neighbours = new[]
                {Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,};

            public static bool HasNeighbour<T>(Vector2Int pnt, T[,] map, Func<Vector2Int, bool> check)
            {
                foreach (var ngb in Neighbours)
                    if (check(ngb))
                        return true;
                return false;
            }

            public static List<Vector2Int> GetRectNeighbours(RectInt src, RectInt globalBounds)
            {
                List<Vector2Int> result = new();
                var yMin = src.yMin - 1;
                if (yMin >= 0)
                    for (var x = src.xMin; x < src.xMax; x++)
                        result.Add(new Vector2Int(x, yMin));

                var yMax = src.yMax ;
                if (yMax < globalBounds.yMax)
                    for (var x = src.xMin; x < src.xMax; x++)
                        result.Add(new Vector2Int(x, yMax));

                var xMin = src.xMin - 1;
                if (xMin >= 0)
                    for (var y = src.yMin; y < src.yMax; y++)
                        result.Add(new Vector2Int(xMin, y));

                var xMax = src.xMax ;
                if (xMax < globalBounds.xMax)
                    for (var y = src.yMin; y < src.yMax; y++)
                        result.Add(new Vector2Int(xMax, y));

                return result;
            }

            public static RectInt ExtendRect(RectInt src)
            {
                return new RectInt(src.position - Vector2Int.one, src.size + Vector2Int.one);
            }

            public static List<Vector2Int> GetAllPointsInRange(RectInt range, RectInt globalBounds)
            {
                List<Vector2Int> result = new();
                for (var x = range.xMin; x < range.xMax; x++)
                {
                    if (x < globalBounds.xMin || x >= globalBounds.xMax)
                        continue;
                    for (var y = range.yMin; y < range.yMax; y++)
                    {
                        if (y < globalBounds.yMin || y >= globalBounds.yMax)
                            continue;
                        result.Add(new Vector2Int(x, y));
                    }
                }

                return result;
            }

            public static void ForeachPointsInRange<T>(RectInt range, T[,] table, Action<int, int> action)
            {
                var globalBounds = GetTableSize(table);
                for (var x = range.xMin; x < range.xMax; x++)
                {
                    if (x < 0 || x >= globalBounds.x)
                        continue;
                    for (var y = range.yMin; y < range.yMax; y++)
                    {
                        if (y < 0 || y >= globalBounds.y)
                            continue;
                        action(x, y);
                    }
                }
            }

            public static Vector2Int GetRandomPoint(Vector2Int size)
            {
                return new Vector2Int(Random.Range(0, size.x), Random.Range(0, size.y));
            }

            public static void ShuffleArray<T>(IList<T> arr)
            {
                for (var i = 0; i < arr.Count; i++)
                {
                    var rnd = Random.Range(0, arr.Count);
                    (arr[i], arr[rnd]) = (arr[rnd], arr[i]);
                }
            }

            public static Vector2Int GetTableSize<T>(T[,] table)
            {
                return new Vector2Int(table.GetLength(0), table.GetLength(1));
            }
        }
    }

    [Serializable]
    public struct DungeonTilesInfo
    {
        public int tilesCount;
        public AreaPrice[] areas;
    }

    [Serializable]
    public struct AreaPrice : IHasWeight
    {
        public string area;
        public float weight;
        public float Weight => weight;
    }
}