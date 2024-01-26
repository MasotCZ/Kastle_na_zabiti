using UnityEngine;

namespace Game.Utility.Tiles
{
    public sealed class SingleTileGridMap : GridTileMapBase
    {
        public override PlacementResponse SetTileAt(Vector3 worldPosition, IGameTile tile)
        {
            var response = BeforePlacing(worldPosition, tile);
            if (response.DuplicateRequestMade == true)
            {
                return response;
            }

            TileMap.ClearAllCells();
            response.Place();
            return response;
        }
    }
}

