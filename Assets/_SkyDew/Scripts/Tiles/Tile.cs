using UnityEngine;

namespace SkyDew.Tiles
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer tileRenderer;
        

        public void SetSprite(Sprite sprite) => tileRenderer.sprite = sprite;
        public void SetColor(Color col) => tileRenderer.color = col;
    }
}