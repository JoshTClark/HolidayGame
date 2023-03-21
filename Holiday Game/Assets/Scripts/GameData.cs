using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int id;
    public float gamePlayTime = 0.0f;
    public int currency = 0;
    private Dictionary<MetaUpgrade.MetaUpgradeID, int> metaUpgrades = new Dictionary<MetaUpgrade.MetaUpgradeID, int>();

    public void init()
    {
        if (metaUpgrades == null)
        {
            metaUpgrades = new Dictionary<MetaUpgrade.MetaUpgradeID, int>();
        }
    }

    public int GetUpgradeLevel(MetaUpgrade.MetaUpgradeID id)
    {
        if (metaUpgrades.ContainsKey(id))
        {
            int output = 0;
            metaUpgrades.TryGetValue(id, out output);
            return output;
        }

        Debug.Log($"Upgrade key not found - Key: {id}\nCreating save data key");
        metaUpgrades.Add(id, 0);
        return 0;
    }

    public void SetUpgrades(List<MetaUpgrade> upgrades)
    {
        metaUpgrades.Clear();
        foreach (MetaUpgrade u in upgrades)
        {
            metaUpgrades.Add(u.id, u.level);
        }
    }
}
