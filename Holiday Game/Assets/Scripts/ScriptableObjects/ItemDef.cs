using System.Collections;
using System.Collections.Generic;
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

    public List<LevelDescription> levelDescriptions = new List<LevelDescription>();

    public Item GetItem()
    {
        Item i = new Item();
        i.itemDef = this;
        i.Level = 1;
        return i;
    }

    [System.Serializable]
    public class LevelDescription
    {
        public int level;
        [TextArea(15, 20)]
        public string desc;
    }
}
