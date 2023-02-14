using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceManager;

[CreateAssetMenu(fileName = "Upgrade Config", menuName = Constants.ASSET_MENU_PATH + "Upgrade Config")]
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
    [TextArea(15, 20)]
    public string upgradeDescription;
    public ResourceManager.UpgradeIndex index;
    public Tier tier;
    public bool CanTakeMultiple;
    public bool IsWeapon;
    public Sprite icon;

    public StatChange statChange;
    public WeaponIndex weaponStatIndex;
    public List<WeaponStatChange> weaponStatChanges = new List<WeaponStatChange>();


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

    public WeaponStatChange GetWeaponStatChange(string stat)
    {
        foreach (WeaponStatChange w in weaponStatChanges)
        {
            if (w.stat == stat) {
                return w;
            }
        }

        WeaponStatChange weaponStatChange = new WeaponStatChange();

        weaponStatChange.mult = 1;
        weaponStatChange.add = 0;

        if (stat == "Damage" || stat == "Attack Speed" || stat == "Size") 
        {
            weaponStatChange.isPercent = true;
        }

        return weaponStatChange;
    }

    [System.Serializable]
    public class StatChange
    {
        public float hpAdd, speedAdd, damageAdd, attackSpeedAdd, armorAdd, regenAdd, critChanceAdd, critDamageAdd;
        public float hpMult = 1, speedMult = 1, damageMult = 1, attackSpeedMult = 1, armorMult = 1, regenMult = 1, critChanceMult = 1, critDamageMult = 1, XPMult = 1;
    }

    [System.Serializable]
    public class WeaponStatChange
    {
        [SerializeField]
        public string stat = "";
        [SerializeField]
        public float add = 0f;
        [SerializeField]
        public float mult = 1f;
        [SerializeField]
        public bool isPercent = false;
    }
}
