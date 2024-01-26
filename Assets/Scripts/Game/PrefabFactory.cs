using UnityEngine;

public static class PrefabFactory
{
    public static GameObject CreateAt(Vector3 position, GameObject prefab)
    {
        var res = GameObject.Instantiate(prefab);
        res.transform.position = position;
        return res;
    }
}
