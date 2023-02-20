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

    public bool HasTakenPath(string pathName)
    {
        if (currentPath.pathName == pathName)
        {
            return true;
        }
        foreach (UpgradePath p in takenPaths)
        {
            if (p.pathName == pathName)
            {
                return true;
            }
        }
        return false;
    }
}
