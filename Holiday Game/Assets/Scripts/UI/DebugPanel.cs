using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ResourceManager;

public class DebugPanel : MonoBehaviour
{
    public TMP_Dropdown upgradeSelector;
    public TMP_Dropdown enemySelector;
    public TMP_Dropdown dropSelector;
    public TMP_Text inventoryDisplay;

    public void Init()
    {
        List<TMP_Dropdown.OptionData> optionsUpgrades = new List<TMP_Dropdown.OptionData>();
        foreach (Upgrade i in ResourceManager.upgradeDefinitions)
        {
            optionsUpgrades.Add(new TMP_Dropdown.OptionData(i.upgradeName));
        }
        upgradeSelector.AddOptions(optionsUpgrades);

        List<TMP_Dropdown.OptionData> optionsEnemies = new List<TMP_Dropdown.OptionData>();
        foreach (Enemy i in ResourceManager.enemyPrefabs)
        {
            optionsEnemies.Add(new TMP_Dropdown.OptionData(i.name));
        }
        enemySelector.AddOptions(optionsEnemies);

        List<TMP_Dropdown.OptionData> optionsDrops = new List<TMP_Dropdown.OptionData>();
        foreach (DropBase i in ResourceManager.pickupPrefabs)
        {
            optionsDrops.Add(new TMP_Dropdown.OptionData(i.name));
        }
        dropSelector.AddOptions(optionsDrops);
    }

    private void Update()
    {
        inventoryDisplay.text = "";
        foreach (Item i in GameManager.instance.Player.inventory)
        {
            inventoryDisplay.text += i.itemDef.itemName + ": LVL " + i.Level + "\n";
        }
    }

    public void OnEnable()
    {
        Cursor.visible = true;
    }

    public void OnDisable()
    {
        Cursor.visible = false;
    }

    public void GiveUpgrade()
    {
        UpgradeIndex index = ResourceManager.UpgradeIndexFromName(upgradeSelector.options[upgradeSelector.value].text);
        Debug.Log("Giving player " + ResourceManager.GetUpgrade(index).upgradeName);
        /*
        GameManager.instance.Player.AddUpgrade(index);
        */
    }

    public void SpawnEnemy()
    {
        Enemy e = ResourceManager.EnemyFromName(enemySelector.options[enemySelector.value].text);
        Debug.Log("Spawning " + e.name);
        EnemyManager.instance.SpawnEnemy(e.index);
    }

    public void SpawnDrop()
    {
        Debug.Log("Spawning " + ResourceManager.DropFromName(dropSelector.options[dropSelector.value].text).name);
        DropBase drop = DropManager.GetPickup(ResourceManager.DropFromName(dropSelector.options[dropSelector.value].text).index);
        drop.gameObject.transform.position = new Vector2(GameManager.instance.Player.transform.position.x, GameManager.instance.Player.transform.position.y + 10);
    }
}
