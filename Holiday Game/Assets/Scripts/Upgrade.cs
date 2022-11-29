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
        Rare,
        Epic,
        Legendary
    }

    public string upgradeName;
    [TextArea (15, 20)]
    public string upgradeDescription;
    public ResourceManager.UpgradeIndex index;
    public Tier tier;
    public bool CanTakeMultiple;
    public bool IsWeapon;
    public Sprite icon;

    public StatChange statChange;


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

    [System.Serializable]
    public class StatChange 
    {
        public float hpAdd, speedAdd, damageAdd, attackSpeedAdd, armorAdd, regenAdd, critChanceAdd, critDamageAdd;
        public float hpMult = 1, speedMult = 1, damageMult = 1, attackSpeedMult = 1, armorMult = 1, regenMult = 1, critChanceMult = 1, critDamageMult = 1;
    }
}
