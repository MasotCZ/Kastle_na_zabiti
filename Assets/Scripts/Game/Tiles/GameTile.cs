using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Utility.Tiles
{
    public class GameTile : TileBase
    {
        public string tileName;
        public Sprite tilePreview;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.sprite = tilePreview;
        }
    }
}

