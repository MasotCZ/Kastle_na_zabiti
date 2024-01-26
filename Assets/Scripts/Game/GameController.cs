using Game.Serialization;
using Game.Utility.Tiles;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public DeployZoneController deployZoneController;
    public GameObjectSpawnGameTile RenderedOjbectTest;

    private void OnEnable()
    {
        GlobalMetaDataContainer.Instance.CreateDictionary();

        deployZoneController.SetSelectedTile(RenderedOjbectTest);
    }
}
