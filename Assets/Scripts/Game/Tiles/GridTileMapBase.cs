using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game.Utility.Tiles
{
    public readonly struct PlacementDto
    {
        public readonly IGameTile tile;
        /// <summary>
        /// anchor position for offsets or the current cell position if no offset
        /// </summary>
        public readonly Vector3Int cellPosition;
        /// <summary>
        /// for multi cell objects
        /// </summary>
        public readonly Vector3Int[] offsets;

        public bool IsSingleTileObject => offsets.Length == 0;

        public PlacementDto(IGameTile tile, Vector3Int position)
        {
            this.tile = tile;
            this.cellPosition = position;
            this.offsets = new Vector3Int[0];
        }

        public PlacementDto(IGameTile tile, Vector3Int position, Vector3Int[] offsets)
        {
            this.tile = tile;
            this.cellPosition = position;
            this.offsets = offsets;
        }

        public bool Equals(PlacementDto obj)
        {
            //Debug.Log($"OBJ1: {obj.cellPosition}|{obj.offsets}|{String.Join(',', obj.offsets.SelectMany(x => x.ToString() + ','))}");

            return cellPosition == obj.cellPosition
                && ((offsets is null
                && obj.offsets is null)
                || (offsets.All(x => obj.offsets.Contains(x))));
            ;
        }
    }

    public interface ITileMap
    {
        public void ClearAllCells();
        public Vector3Int WorldToCell(Vector3 position);
        public void SetTile(PlacementDto placementDto);
        public IGameTileData GetTileData(Vector3Int cellPositon);
    }

    public class TileMap : ITileMap
    {
        private readonly Dictionary<Vector3Int, IGameTileData> _data = new Dictionary<Vector3Int, IGameTileData>();
        private readonly Grid _grid;

        public TileMap(Grid grid)
        {
            _grid = grid;
        }

        private void RemoveIfExists(PlacementDto placementDto)
        {
            var data = GetTileData(placementDto.cellPosition);

            if (data is null)
                return;

            _data.Remove(placementDto.cellPosition);

            foreach (var offset in data.GameTile.Offsets)
            {
                _data.Remove(placementDto.cellPosition + offset);
            }
            data.GameTile.Dispose(data);
        }

        /// <summary>
        /// TODO implement multi offset object
        /// if you click not on cell position itself then it doesnt work
        /// </summary>
        /// <param name="placementDto"></param>
        public void SetTile(PlacementDto placementDto)
        {
            RemoveIfExists(placementDto);

            if (placementDto.tile is null)
            {
                return;
            }

            IGameTileData data = new GameTileData();
            placementDto.tile.OnSpawn(placementDto.cellPosition, this, data);
            _data.Add(placementDto.cellPosition, data);

            foreach (var offset in placementDto.offsets)
            {
                _data.Add(placementDto.cellPosition + offset, data);
            }
        }

        public IGameTileData GetTileData(Vector3Int cellPositon)
        {
            IGameTileData res = null;
            _data.TryGetValue(cellPositon, out res);
            return res;
        }

        public Vector3Int WorldToCell(Vector3 position)
            => _grid.WorldToCell(position + _grid.cellSize / 2);

        public void ClearAllCells()
        {
            foreach (var data in _data.Values)
            {
                data.GameTile.Dispose(data);
            }
            _data.Clear();
        }
    }

    public struct PlacementResponse
    {
        public bool DuplicateRequestMade { get; set; }
        public bool IsValid => InvalidOffsets.Count() == 0;
        public readonly IEnumerable<Vector3Int> ValidOffsets { get; }
        public readonly IEnumerable<Vector3Int> InvalidOffsets { get; }

        private readonly ITileMap _tilemap;
        private readonly PlacementDto _placementDto;

        public PlacementResponse(PlacementDto placementDto, ITileMap tilemap)
        {
            DuplicateRequestMade = false;
            _tilemap = tilemap;
            _placementDto = placementDto;

            var ll = placementDto.offsets.Length + 1;
            int invalids = placementDto.offsets.Length;
            int valids = 0;
            Span<Vector3Int> effOffsets = stackalloc Vector3Int[ll];

            // check pisition itself
            effOffsets[tilemap.GetTileData(placementDto.cellPosition) is null ? valids++ : invalids--] = Vector3Int.zero;

            // check rest of offsets
            foreach (var offset in placementDto.offsets)
            {
                effOffsets[tilemap.GetTileData(offset + placementDto.cellPosition) is null ? valids++ : invalids--] = offset;
            }

            var validOffsets = new Vector3Int[valids];
            var invalidOffsets = new Vector3Int[invalids];

            if (valids != 0)
                effOffsets.Slice(0, valids).CopyTo(validOffsets);
            if (invalids != 0)
                effOffsets.Slice(valids, effOffsets.Length).CopyTo(invalidOffsets);

            ValidOffsets = validOffsets;
            InvalidOffsets = invalidOffsets;
        }

        public void Place()
        {
            _tilemap.SetTile(_placementDto);
        }
    }

    public interface ITileMapController
    {
        public Vector3Int WorldToCell(Vector3 position);
        public PlacementResponse SetTileAt(Vector3 worldPosition, IGameTile tile);
        public IGameTileData GetTileData(Vector3 worldPosition);
    }

    public abstract class GridTileMapBase : MonoBehaviour, ITileMapController
    {
        /// <summary>
        /// grid associated with this tilemap
        /// </summary>
        private ITileMap _tileMap;
        private (PlacementDto placementDto, PlacementResponse placementResponse) _lastModifiedCell;

        protected ITileMap TileMap => _tileMap;

        public void Start()
        {
            _tileMap = new TileMap(this.GetComponentInParent<Grid>());
        }

        protected PlacementDto GetPlacementDto(Vector3 worldPosition, IGameTile tile)
            => new PlacementDto(tile, WorldToCell(worldPosition));

        protected bool IsLastModified(PlacementDto placementDto)
        {
            return placementDto.Equals(_lastModifiedCell.placementDto);
        }

        protected PlacementResponse CanPlace(PlacementDto placementDto)
        {
            if (IsLastModified(placementDto))
            {
                _lastModifiedCell.placementResponse.DuplicateRequestMade = true;
                return _lastModifiedCell.placementResponse;
            }

            _lastModifiedCell = (placementDto, new PlacementResponse(placementDto, _tileMap));
            return _lastModifiedCell.placementResponse;
        }

        protected PlacementResponse BeforePlacing(Vector3 worldPosition, IGameTile tile)
        {
            return CanPlace(GetPlacementDto(worldPosition, tile));
        }

        public virtual PlacementResponse SetTileAt(Vector3 worldPosition, IGameTile tile)
        {
            var response = BeforePlacing(worldPosition, tile);
            if (response.IsValid && !response.DuplicateRequestMade)
            {
                response.Place();
            }

            return response;
        }

        public Vector3Int WorldToCell(Vector3 position)
            => _tileMap.WorldToCell(position);

        public IGameTileData GetTileData(Vector3 worldPosition)
            => _tileMap.GetTileData(WorldToCell(worldPosition));
    }
}

