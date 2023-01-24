using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Playable Character", menuName = Constants.ASSET_MENU_PATH + "Playable Character")]
public class PlayableCharacterData : ScriptableObject
{
    [SerializeField]
    public Sprite sprite;

    [SerializeField]
    public List<Upgrade> inventory = new List<Upgrade>();

    [SerializeField]
    public StatsData stats = new StatsData();
}

[System.Serializable]
public struct StatsData
{
    [SerializeField]
    private float baseMaxHP, baseSpeed, baseDamage, baseAttackSpeed, baseArmor, baseRegen, baseRegenInterval, baseCritChance, baseCritDamage;
}
