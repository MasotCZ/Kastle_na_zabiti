using Game.Serialization;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public DeployZoneController deployZoneController;
    public GenericObjectMetaData RenderedOjbectTest;

    private void OnEnable()
    {
        GlobalMetaDataContainer.Instance.CreateDictionary();

        deployZoneController.SetSelectedTile(RenderedOjbectTest);
    }
}
