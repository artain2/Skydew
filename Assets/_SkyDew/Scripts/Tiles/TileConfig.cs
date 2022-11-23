using UnityEngine;

namespace SkyDew.Tiles
{
    public class TileConfig : ScriptableObject
    {
        public string groundType;
        public Sprite tileSprite;
        public Color tileColor = Color.white;
    }
}