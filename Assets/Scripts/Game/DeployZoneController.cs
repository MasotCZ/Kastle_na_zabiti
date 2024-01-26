using Game.Utility.Tiles;
using Masot.Standard.Editor;
using Masot.Standard.Input;
using Masot.Standard.Input.EventArguments.Mouse.Drag;
using Masot.Standard.Input.EventArguments.Mouse.Position;
using UnityEngine;

public class DeployZoneController : MonoBehaviour
{
    private bool _selected = false;

    public SingleTileGridMap previewTileMap;
    public MultiTileGridMap build1TileMap;

    [SerializeField]
    [ReadOnly]
    private IGameTile _selectedTile;

    public void SetSelectedTile(IGameTile tile)
    {
        if (tile is null)
        {
            _selected = false;
            InputExtensions.MouseDragEventHandler -= OnMouseMove;
            return;
        }

        if (_selected == false)
        {
            InputExtensions.MouseDragEventHandler += OnMouseMove;
            this.RegisterInput<MousePositionEventArgs>(new InputDefine(new InputData(KeyCode.Mouse0)), OnMouseClick);
            _selected = true;
        }

        _selectedTile = tile;

    }

    private void SetTile(Vector3 position)
    {
        previewTileMap.SetTileAt(position, _selectedTile);
    }

    private void SetPreview(Vector3 position)
    {
        previewTileMap.SetTileAt(position, _selectedTile);
    }

    private void OnMouseMove(MouseAxisDragEventArgs e)
    {
        SetPreview(e.WorldPosition);
    }

    private void OnMouseClick(MousePositionEventArgs e)
    {

    }

}