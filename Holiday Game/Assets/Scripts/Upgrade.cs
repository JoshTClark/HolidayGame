using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Config", menuName = "ScriptableObject/Upgrade Config")]
public class Upgrade : ScriptableObject
{
    public enum Tier
    {
        Common,
        Uncommon,
        Rare
    }

    public string upgradeName;
    public string upgradeDescription;
    public GameManager.UpgradeIndex index;
    public Tier tier;
    [Range(1,5)]
    public int levels;
}
