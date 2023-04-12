using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ResourceManager;

public class DebugPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown upgradeSelector, upgradeLevelSelector, enemySelector, dropSelector, characterSelector;

    [SerializeField]
    private TMP_Text inventoryDisplay;

    public void Init()
    {
        List<TMP_Dropdown.OptionData> optionsItems = new List<TMP_Dropdown.OptionData>();
        foreach (ItemDef i in ResourceManager.itemDefs)
        {
            optionsItems.Add(new TMP_Dropdown.OptionData(i.itemName));
        }
        upgradeSelector.AddOptions(optionsItems);

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

        List<TMP_Dropdown.OptionData> optionsCharacters = new List<TMP_Dropdown.OptionData>();
        foreach (PlayableCharacterData i in ResourceManager.characters)
        {
            optionsCharacters.Add(new TMP_Dropdown.OptionData(i.characterName));
        }
        characterSelector.AddOptions(optionsCharacters);
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
        Item item = ResourceManager.ItemDefFromName(upgradeSelector.options[upgradeSelector.value].text).GetItem();
        string rawText = upgradeLevelSelector.options[upgradeLevelSelector.value].text;
        string pathString = rawText.Split("|")[0].Trim();
        ItemDef.UpgradePath path = null;

        foreach (ItemDef.UpgradePath p in item.itemDef.paths)
        {
            //Debug.Log(p.pathName + " = " + pathString);
            if (p.pathName == pathString)
            {
                item.takenPaths.Add(p);
                item.currentPath = p;
                path = p;
            }
        }

        foreach (ItemDef.UpgradePath p in item.itemDef.paths)
        {
            if (p.pathName == path.previousPath)
            {
                item.Level = Int32.Parse(rawText.Split("|")[1].Trim()) + p.numLevels;
            }
        }

        GameManager.instance.Player.AddItem(item);
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

    public void ChangeCharacter()
    {
    }

    public void UpdateUpgradeLevelSelector() 
    {
        ItemDef def = ResourceManager.ItemDefFromName(upgradeSelector.options[upgradeSelector.value].text);

        List<TMP_Dropdown.OptionData> optionsLevels = new List<TMP_Dropdown.OptionData>();

        foreach (ItemDef.UpgradePath i in def.paths)
        {
            for (int k = 0; k < i.levelDescriptions.Count; k++)
            {
                optionsLevels.Add(new TMP_Dropdown.OptionData(i.pathName + " | " + (k + 1)));
            }
        }

        upgradeLevelSelector.AddOptions(optionsLevels);
    }
}
