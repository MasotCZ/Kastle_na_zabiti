using Game.Serialization;
using UnityEngine;

namespace Game.Utility.Tiles
{
    public interface IGameTile
    {
        public string Name { get; }
        public string Description { get; }
        public Vector3Int[] Offsets { get; }
        public IGameTileData OnSpawn(Vector3Int position, ITileMap tilemap, IGameTileData data);
        public IGameTileData GetTileData(Vector3Int position, ITileMap tilemap, IGameTileData data);
        public void Dispose(IGameTileData data);
    }

    public abstract class GameTileBase : RenderedObjectMetaDataBase, IGameTile
    {
        public Vector3Int[] Offsets { get; } = new Vector3Int[0];
        public abstract void Dispose(IGameTileData data);
        public abstract IGameTileData GetTileData(Vector3Int position, ITileMap tilemap, IGameTileData data);
        public IGameTileData OnSpawn(Vector3Int position, ITileMap tilemap, IGameTileData data)
        {
            return OnSpawn(position, tilemap, data, GameObjectPrefab);
        }
        protected abstract IGameTileData OnSpawn(Vector3Int position, ITileMap tilemap, IGameTileData data, GameObject prefab);
    }
}

