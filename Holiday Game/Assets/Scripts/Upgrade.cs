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
    public ResourceManager.UpgradeIndex index;
    public Tier tier;
    /*
    [Range(0, 1), Tooltip("Overrides the weight given by the tier. Leave at 1 to not have it overriden.")]
    public float weight;
    */
    public bool CanTakeMultiple;

    private int currentLevel = 1;
    public int CurrentLevel
    {
        get
        {
            return currentLevel;
        }
        set
        {
            currentLevel = value;
        }
    }
}
