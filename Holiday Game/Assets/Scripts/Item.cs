using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public ItemDef itemDef;
    private int level = 0;

    public int Level
    {
        get { return level; }
        set { level = value; }
    }
}
