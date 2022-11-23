using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using DrawerTools;
using Sirenix.OdinInspector;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace SkyDew.Tiles
{
    [ExecuteInEditMode]
    public class TileDrawer : MonoBehaviour
    {
        [SerializeField] private TileGenerator generator;
        [SerializeField] private TileConfig config;

        [SerializeField, ValueDropdown("MaskNames")]
        private string _mask;

        private bool _mainMouseButtonPressed;
        private bool _secondMouseButtonPressed;
        private bool _insideWindow = false;
        private CompositeDisposable _disposable = new CompositeDisposable();
        private bool _recording = false;

        private bool Recording => _recording;
        private bool NotRecording => !_recording;
        private static string[] MaskNames => Masks.Keys.ToArray();

        private PointMask ActiveMask => Masks[_mask];


        private static Dictionary<string, PointMask> Masks = new Dictionary<string, PointMask>()
        {
            {"Single", new PointMask(new[,] {{1}})},
            {"Rect2", new PointMask(new[,] {{1, 1}, {1, 1}})},
            {
                "Rect3", new PointMask(new[,]
                {
                    {1, 1, 1},
                    {1, 1, 1},
                    {1, 1, 1},
                })
            },
            {
                "Soft3", new PointMask(new[,]
                {
                    {0, 1, 0},
                    {1, 1, 1},
                    {0, 1, 0},
                })
            },
            {
                "Rect5", new PointMask(new[,]
                {
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1},
                })
            },
            {
                "Rect15", new PointMask(new[,]
                {
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
                })
            },
        };


        private void OnEnable()
        {
            _recording = false;
            SceneView.duringSceneGui -= AtSceneViewBrush;

            if (Masks.TryGetValue(_mask, out var mask))
                _mask = Masks.First().Key;
        }

        [Button, EnableIf("NotRecording")]
        void StartRecord()
        {
            SceneView.duringSceneGui -= AtSceneViewBrush;
            SceneView.duringSceneGui += AtSceneViewBrush;
            _recording = true;
        }

        [Button, EnableIf("Recording")]
        void StopRecord()
        {
            SceneView.duringSceneGui -= AtSceneViewBrush;
            _disposable.Clear();
            _recording = false;
        }

        private void AtSceneViewBrush(SceneView view)
        {
            Event e = Event.current;

            if (Event.current.type == EventType.MouseLeaveWindow)
            {
                _insideWindow = false;
                _mainMouseButtonPressed = false;
                _secondMouseButtonPressed = false;
                return;
            }

            if (Event.current.type == EventType.MouseEnterWindow)
            {
                _insideWindow = true;
                return;
            }

            var controlID =
                GUIUtility.GetControlID(FocusType
                    .Passive); // Это стандартный контрол мыши от юнити. Он нам не нужен пока кисть активна

            if (!e.isMouse)
                return;

            if (e.type == EventType.MouseDown &&
                _insideWindow) // Проглатываем контрол, чтобы небыло стандартной юнити реакции на мышь
            {
                GUIUtility.hotControl = 0;
                e.Use();
                _mainMouseButtonPressed = e.button == 0;
                _secondMouseButtonPressed = e.button == 1;
            }

            if (e.type == EventType.MouseUp && _insideWindow) // Возвращаем контрол
            {
                _mainMouseButtonPressed = false;
                _secondMouseButtonPressed = false;
                GUIUtility.hotControl = controlID;
                e.Use();
            }

            if (!_insideWindow)
            {
                GUIUtility.hotControl = 0;
                Selection.activeObject = gameObject;
            }

            BrushDraw(view);
        }

        private void BrushDraw(SceneView view)
        {
            if (!_mainMouseButtonPressed && !_secondMouseButtonPressed)
                return;

            var pos = Event.current.mousePosition;
            pos.y = view.camera.pixelHeight - pos.y;
            pos = view.camera.ScreenToWorldPoint(pos);

            if (_mainMouseButtonPressed)
            {
                if (!generator.TryGetPointByPosition(pos, out var point))
                    return;
                var maskPoints = ActiveMask.GetPoints(point);
                foreach (var pnt in maskPoints)
                {
                    generator.SetTileInfo(pnt.x, pnt.y, config);
                }
            }
        }

        public class PointMask
        {
            private Vector2Int _center;
            private RectInt _rect;
            private int[,] _values;

            public PointMask(int[,] values)
            {
                _values = values;
                var xMax = _values.GetUpperBound(0);
                var yMax = _values.GetUpperBound(1);
                _rect = new RectInt(0, 0, xMax, yMax);
                _center = new Vector2Int(xMax / 2, yMax / 2);
            }

            public List<Vector2Int> GetPoints(Vector2Int src)
            {
                var result = new List<Vector2Int>();
                for (var j = 0; j <= _rect.width; j++)
                for (var i = 0; i <= _rect.height; i++)
                    if (_values[j, i] != 0)
                        result.Add(new Vector2Int(i, j) - _center + src);
                return result;
            }
        }
    }
}