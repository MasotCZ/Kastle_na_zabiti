using Game.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Utility.Tiles
{
    public readonly struct PlacementDto
    {
        public readonly GameObject prefab;
        /// <summary>
        /// anchor position for offsets or the current cell position if no offset
        /// </summary>
        public readonly Vector3Int position;
        /// <summary>
        /// for multi cell objects
        /// </summary>
        public readonly Vector3Int[] offsets;

        public bool IsSingleTileObject => offsets.Length == 0;

        public PlacementDto(GameObject prefab, Vector3Int position, Vector3Int[] offsets = null)
        {
            this.prefab = prefab;
            this.position = position;
            this.offsets = offsets;
        }
    }

    [RequireComponent(typeof(Tilemap))]
    public sealed class GridTileMap : MonoBehaviour
    {
        public Grid layoutGrid => Tilemap.layoutGrid;

        private Tilemap _tilemap;
        private Tilemap Tilemap
        {
            get
            {
                if (_tilemap is null)
                {
                    _tilemap = GetComponent<Tilemap>();
                }

                return _tilemap;
            }
        }

        // assumes palcement in the middle
        public Vector3Int WorldToCell(Vector3 position) => _tilemap.WorldToCell(position + _tilemap.layoutGrid.cellSize / 2);

        /// <summary>
        /// Add an object to position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="transform"></param>
        private PlacementResponse AddObject(PlacementDto placementDto)
        {

        }

        private void RemoveObject(PlacementDto placementDto)
        {

        }

        public struct PlacementResponse
        {
            public static PlacementResponse Default = new PlacementResponse(new Vector3Int[0], new Vector3Int[0]);
            public bool IsValid => InvalidOffsets.Count() == 0;
            public IEnumerable<Vector3Int> ValidOffsets { get; }
            public IEnumerable<Vector3Int> InvalidOffsets { get; }

            public PlacementResponse(IEnumerable<Vector3Int> validOffsets, IEnumerable<Vector3Int> invalidOffsets)
            {
                this.ValidOffsets = validOffsets;
                this.InvalidOffsets = invalidOffsets;
            }
        }

        public PlacementResponse TryPlace(PlacementDto obj)
        {
            var ll = obj.offsets.Length + 1;
            Span<Vector3Int> effOffsets = stackalloc Vector3Int[ll];
            int invalids = ll;
            int valids = 0;

            // check pisition itself
            effOffsets[GetGameObject(obj.position) is null ? valids++ : invalids--] = Vector3Int.zero;

            // check rest of offsets
            foreach (var offset in obj.offsets)
            {
                effOffsets[GetGameObject(offset + obj.position) is null ? valids++ : invalids--] = offset;
            }

            var validOffsets = new Vector3Int[valids];
            var invalidOffsets = new Vector3Int[invalids];

            var valid = effOffsets.Slice(0, valids);
            var invalid = effOffsets.Slice(valids, effOffsets.Length);

            valid.CopyTo(validOffsets);
            invalid.CopyTo(invalidOffsets);

            return new PlacementResponse(validOffsets, invalidOffsets);
        }

        /// <summary>
        /// check if u can add an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool CanAdd(PlacementDto obj)
        {
            var pos = obj.position + obj.offsets[0];
            var baseLine = GetTile(pos).GetHeight(pos);

            foreach (var item in obj.offsets)
            {
                pos = item + obj.position;

                if (GetGameObject(pos) is not null)
                {
                    return false;
                }

                if (baseLine != GetTile(pos).GetHeight(pos))
                {
                    return false;
                }
            }

            return true;
        }

        public GridMapTile GetTile(Vector3Int index) => Tilemap.GetTile(index) as GridMapTile;
        public void SetTile(Vector3Int index, GridMapTile tile) => Tilemap.SetTile(index, tile);


        public void SetTileAt(Vector3 worldPosition, IRenderedObjectMeta meta)
        {
            AddObject(new PlacementDto(meta.GameObjectPrefab, WorldToCell(worldPosition)));
        }

        public void RemoveTileAt(Vector3 worldPosition)
        {
            var cellPosition = WorldToCell(worldPosition);
            RemoveObject()
        }
    }
}

