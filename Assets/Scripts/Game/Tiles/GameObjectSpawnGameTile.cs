using UnityEngine;

namespace Game.Utility.Tiles
{
    public interface IGameTileData
    {
        public GameObject GameObject { get; set; }
        public bool Disposed { get; set; }
        public IGameTile GameTile { get; set; }
    }

    public class GameTileData : IGameTileData
    {
        public GameObject GameObject { get; set; } = null;
        public bool Disposed { get; set; } = false;
        public IGameTile GameTile { get; set; } = null;
    }

    [CreateAssetMenu(fileName = "GameObjectTile", menuName = "Tiles/GameObjectTile")]
    public class GameObjectSpawnGameTile : GameTileBase
    {
        public override void Dispose(IGameTileData data)
        {
            if (data.Disposed)
            {
                return;
            }

            _Dispose(data);
        }

        private void _Dispose(IGameTileData data)
        {
            if (data.GameObject is not null)
                GameObject.Destroy(data.GameObject);
            data.Disposed = true;
        }

        public override IGameTileData GetTileData(Vector3Int position, ITileMap tilemap, IGameTileData data)
        {
            return data;
        }

        protected override IGameTileData OnSpawn(Vector3Int position, ITileMap tilemap, IGameTileData data, GameObject prefab)
        {
            data.GameObject = PrefabFactory.CreateAt(position, prefab);
            data.Disposed = false;
            data.GameTile = this;
            return data;
        }
    }
}

