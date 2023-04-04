using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static ResourceManager;

[CreateAssetMenu(fileName = "Item Definition", menuName = Constants.ASSET_MENU_PATH + "Item Definition")]
public class ItemDef : ScriptableObject
{
    public enum Tier
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4
    }

    public string itemName;
    public ItemIndex index;
    public Sprite icon;
    public int maxLevel;
    public Tier tier;

    [Header("Upgrade Paths")]
    public List<UpgradePath> paths = new List<UpgradePath>();

    /// <summary>
    /// Gets a new item of level zero
    /// </summary>
    /// <returns></returns>
    public Item GetItem()
    {
        Item i = new Item();
        i.itemDef = this;
        i.Level = 0;
        if (paths.Count > 0)
        {
            i.currentPath = paths[0];
        }
        return i;
    }

    [System.Serializable]
    public class LevelDescription
    {
        public string lvlName;
        [TextArea(10, 20)]
        public string desc;
        public List<string> statChanges = new List<string>();
    }

    [System.Serializable]
    public class UpgradePath
    {
        public string pathName;
        public string previousPath;
        public int numLevels;
        public List<LevelDescription> levelDescriptions = new List<LevelDescription>();
    }
}
