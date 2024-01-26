using Game.Serialization;
using Game.Utility.Tiles;
using Masot.Standard.Input;
using Masot.Standard.Input.EventArguments.Mouse.Drag;
using UnityEngine;

public class DeployZoneController : MonoBehaviour
{
    private bool selected = false;
    private IRenderedObjectMeta selectedMeta;
    private Vector3Int[] selectedOffsets;
    private Vector3Int previewCell;

    private PlacementDto previewObj;
    private bool init = false;

    public GridTileMap previewTileMap;
    public GridTileMap build1TileMap;

    private void SomeRandomShit(IRenderedObjectMeta meta)
    {
        var sr = meta.GameObjectPrefab.GetComponentInChildren<SpriteRenderer>();

        var worldWidth = sr.sprite.rect.width / sr.sprite.pixelsPerUnit;
        var worldHeight = sr.sprite.rect.height / sr.sprite.pixelsPerUnit;

        var cellWidth = Mathf.CeilToInt(worldWidth / previewTileMap.layoutGrid.cellSize.x);
        var cellHeight = Mathf.CeilToInt(worldHeight / previewTileMap.layoutGrid.cellSize.y);

        var offsets = cellWidth * cellHeight;
        selectedOffsets = new Vector3Int[offsets];

        for (int i = 0; i < offsets; i++)
        {
            selectedOffsets[i] = new Vector3Int(i % cellWidth, i / cellWidth, 0);
        }
    }

    public void SetSelectedTile(IRenderedObjectMeta meta)
    {
        if (meta is null)
        {
            selected = false;
            InputExtensions.MouseDragEventHandler -= OnMouseMove;
            return;
        }

        if (selected == false)
        {
            InputExtensions.MouseDragEventHandler += OnMouseMove;
            selected = true;
        }

        selectedMeta = meta;

    }

    private void RemoveTile(Vector3 position, GridTileMap gridTileMap)
    {

    }

    private void SetTile(Vector3 position, GridTileMap gridTileMap)
    {
        var cellPosition = previewTileMap.WorldToCell(position);

        if (cellPosition == previewCell)
        {
            return;
        }

        previewCell = cellPosition;

        if (init)
        {
            previewTileMap.RemoveObject(previewObj);
        }

        previewObj = new PlacementDto(selectedMeta.GameObjectPrefab, cellPosition, selectedOffsets);
        previewTileMap.AddObject(previewObj);
        init = true;
    }

    private void SetPreview(Vector3 position)
    {
        var cell = previewTileMap.WorldToCell(position + previewTileMap.layoutGrid.cellSize / 2);

        if (cell == previewCell)
        {
            return;
        }

        previewCell = cell;

        if (init)
        {
            previewTileMap.RemoveObject(previewObj);
        }

        previewObj = new PlacementDto(selectedMeta.GameObjectPrefab, cell, selectedOffsets);
        previewTileMap.AddObject(previewObj);
        init = true;
    }

    private void OnMouseMove(MouseAxisDragEventArgs e)
    {
        SetPreview(e.WorldPosition);
    }

}