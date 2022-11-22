using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public Vector2 minCoords;
    public Vector2 maxCoords;
    public List<LevelObjectData> objectDatas = new List<LevelObjectData>();
    private List<GameObject> objects = new List<GameObject>();

    public void CreateLevel()
    {
        foreach (LevelObjectData data in objectDatas)
        {
            for (int i = 0; i < data.numObjs; i++)
            {
                GameObject obj = Instantiate<GameObject>(data.prefab, this.gameObject.transform);
                obj.transform.position = new Vector2(Random.Range(minCoords.x, maxCoords.x), Random.Range(minCoords.y, maxCoords.y));
                objects.Add(obj);
            }
        }
    }

    public void Clean()
    {
        for (int i = objects.Count - 1; i >= 0; i--) 
        {
            Destroy(objects[i]);
        }
        objects.Clear();
    }
}

[System.Serializable]
public class LevelObjectData
{
    public GameObject prefab;
    public float numObjs;
}
