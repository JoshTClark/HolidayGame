using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPoint : MonoBehaviour
{
    public LevelData level;
    public bool isLocked;
    public bool isComplete;
    public SessionMap.MapNode node;
    public Sprite[] status;

    public void Start()
    {
        if (isLocked)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = status[0];
        }

        if (isComplete)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = status[1];
        }
    }
}
