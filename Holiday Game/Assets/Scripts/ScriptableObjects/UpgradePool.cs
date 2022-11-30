using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Pool Config", menuName = Constants.ASSET_MENU_PATH + "Upgrade Pool Config")]
public class UpgradePool : ScriptableObject
{
    public ResourceManager.UpgradePoolIndex index;
    public List<ResourceManager.UpgradeIndex> upgrades = new List<ResourceManager.UpgradeIndex>();
}
