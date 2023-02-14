using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemDef;

public class Item
{
    public ItemDef itemDef;
    private int level = 0;
    public UpgradePath currentPath;
    public List<UpgradePath> takenPaths = new List<UpgradePath>();

    public int Level
    {
        get { return level; }
        set { level = value; }
    }
}
