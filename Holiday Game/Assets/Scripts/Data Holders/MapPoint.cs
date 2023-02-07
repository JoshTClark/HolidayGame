using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoint : MonoBehaviour
{
    public LevelData level;
    public bool isLocked;
    public bool isComplete;
    public SessionMap.MapNode node;

    public void Start()
    {
        if (isLocked)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.5f);
        }

        if (isComplete)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 1f);
        }
    }
}
