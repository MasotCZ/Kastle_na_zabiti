using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Utility.Tiles
{
    [CreateAssetMenu(fileName = "GridTile", menuName = "Tiles/GridTile")]
    public sealed class GridMapTile : GameTile
    {
        //public PerlinNoiseGenerator generator;
        public Gradient gradient;
        public float min;
        public float max;

        private struct GridTileData
        {
            public readonly int height;
            public readonly Color color;

            public GridTileData(int height, Color color)
            {
                this.height = height;
                this.color = color;
            }
        }

        private class GridTileDataContainer : Dictionary<Vector3Int, GridTileData> { }

        private readonly GridTileDataContainer data = new GridTileDataContainer();

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            if (!data.ContainsKey(position))
            {
                InitTileData(position);
            }

            tileData.color = data[position].color;
            base.GetTileData(position, tilemap, ref tileData);
        }

        private void InitTileData(Vector3Int position)
        {
            //var height = Mathf.RoundToInt(generator.GenerateHeight(position.x, position.y) * max);
            //var color = gradient.Evaluate((height - min) / (max - min));

            //data.Add(position, new GridTileData(height, color));
        }

        public int GetHeight(Vector3Int position) => data[position].height;
        public Color GetColor(Vector3Int position) => data[position].color;
    }
}

