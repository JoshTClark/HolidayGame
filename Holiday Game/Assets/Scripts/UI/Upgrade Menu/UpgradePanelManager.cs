using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelManager : MonoBehaviour
{
    public List<UpgradeOption> upgradeOptions = new List<UpgradeOption>();
    public Player player;
    public bool selected = false;
    public bool displaying = false;
    public float commonOdds, uncommonOdds, rareOdds, epicOdds, legendaryOdds;
    public TMP_Text baseStatsTxt;
    public Button rerollButton;
    public CanvasRenderer statsPanel;
    private Chest chest;

    private bool doNewItemsOnNormalLevelUp = true;
    private int numItemsDisplay = 4;

    private void Start()
    {
        chest = null;
        rerollButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets upgrades for the player to choose based on the picked up chest
    /// </summary>
    public void SetUpgrades(Chest c)
    {
        chest = c;

        List<ItemDef.LevelDescription> availableDescsAll = new List<ItemDef.LevelDescription>();
        List<Item> availableAll = new List<Item>();
        List<ItemDef.UpgradePath> pathsAll = new List<ItemDef.UpgradePath>();
        List<ItemDef.LevelDescription> availableDescsWeapons = new List<ItemDef.LevelDescription>();
        List<Item> availableWeapons = new List<Item>();
        List<ItemDef.UpgradePath> pathsWeapons = new List<ItemDef.UpgradePath>();
        List<ItemDef.LevelDescription> availableDescsItems = new List<ItemDef.LevelDescription>();
        List<Item> availableItems = new List<Item>();
        List<ItemDef.UpgradePath> pathsItems = new List<ItemDef.UpgradePath>();

        List<ItemDef> itemDefs = ResourceManager.itemDefs;
        foreach (ItemDef i in itemDefs)
        {
            if (!player.HasItem(i.index))
            {
                Item item = i.GetItem();
                if (i.paths.Count > 0)
                {
                    item.currentPath = i.paths[0];
                    item.Level = 0;
                    availableDescsAll.Add(item.currentPath.levelDescriptions[0]);
                    availableAll.Add(item);
                    pathsAll.Add(item.currentPath);

                    if (i.GetType() == typeof(WeaponDef))
                    {
                        availableDescsWeapons.Add(item.currentPath.levelDescriptions[0]);
                        availableWeapons.Add(item);
                        pathsWeapons.Add(item.currentPath);
                    }
                    else
                    {
                        availableDescsItems.Add(item.currentPath.levelDescriptions[0]);
                        availableItems.Add(item);
                        pathsItems.Add(item.currentPath);
                    }
                }
            }
        }

        // Randomly adds upgrades to the current choices based on the chests contents
        if (availableItems.Count > 0)
        {
            for (int i = 0; i < chest.contents.Count; i++)
            {
                if (chest.contents[i].contentType == Chest.ChestContent.ChestContentType.Preset)
                {
                    Item item = chest.contents[i].presetItem.GetItem();
                    ItemDef.LevelDescription lvlDesc = item.currentPath.levelDescriptions[0];
                    ItemDef.UpgradePath path = item.currentPath;
                    if (availableAll.Contains(item))
                    {
                        availableDescsAll.Remove(lvlDesc);
                        availableAll.Remove(item);
                        pathsAll.Remove(path);
                    }
                    if (availableWeapons.Contains(item))
                    {
                        availableDescsWeapons.Remove(lvlDesc);
                        availableWeapons.Remove(item);
                        pathsWeapons.Remove(path);
                    }
                    if (availableItems.Contains(item))
                    {
                        availableDescsItems.Remove(lvlDesc);
                        availableItems.Remove(item);
                        pathsItems.Remove(path);
                    }
                    AddItem(item, lvlDesc, path, upgradeOptions[i]);
                }
                else if (chest.contents[i].contentType == Chest.ChestContent.ChestContentType.RandomAll)
                {
                    int random = Random.Range(0, availableAll.Count);
                    Item item = availableAll[random];
                    ItemDef.LevelDescription lvlDesc = availableDescsAll[random];
                    ItemDef.UpgradePath path = pathsAll[random];
                    availableDescsAll.RemoveAt(random);
                    availableAll.RemoveAt(random);
                    pathsAll.RemoveAt(random);
                    if (availableWeapons.Contains(item))
                    {
                        availableDescsWeapons.Remove(lvlDesc);
                        availableWeapons.Remove(item);
                        pathsWeapons.Remove(path);
                    }
                    if (availableItems.Contains(item))
                    {
                        availableDescsItems.Remove(lvlDesc);
                        availableItems.Remove(item);
                        pathsItems.Remove(path);
                    }
                    AddItem(item, lvlDesc, path, upgradeOptions[i]);
                }
                else if (chest.contents[i].contentType == Chest.ChestContent.ChestContentType.RandomWeapon)
                {
                    int random = Random.Range(0, availableWeapons.Count);
                    Item item = availableWeapons[random];
                    ItemDef.LevelDescription lvlDesc = availableDescsWeapons[random];
                    ItemDef.UpgradePath path = pathsWeapons[random];
                    availableDescsWeapons.RemoveAt(random);
                    availableWeapons.RemoveAt(random);
                    pathsWeapons.RemoveAt(random);
                    if (availableAll.Contains(item))
                    {
                        availableDescsAll.Remove(lvlDesc);
                        availableAll.Remove(item);
                        pathsAll.Remove(path);
                    }
                    if (availableItems.Contains(item))
                    {
                        availableDescsItems.Remove(lvlDesc);
                        availableItems.Remove(item);
                        pathsItems.Remove(path);
                    }
                    AddItem(item, lvlDesc, path, upgradeOptions[i]);
                }
                else if (chest.contents[i].contentType == Chest.ChestContent.ChestContentType.RandomItem)
                {
                    int random = Random.Range(0, availableItems.Count);
                    Item item = availableItems[random];
                    ItemDef.LevelDescription lvlDesc = availableDescsItems[random];
                    ItemDef.UpgradePath path = pathsItems[random];
                    availableDescsItems.RemoveAt(random);
                    availableItems.RemoveAt(random);
                    pathsItems.RemoveAt(random);
                    if (availableAll.Contains(item))
                    {
                        availableDescsAll.Remove(lvlDesc);
                        availableAll.Remove(item);
                        pathsAll.Remove(path);
                    }
                    if (availableWeapons.Contains(item))
                    {
                        availableDescsWeapons.Remove(lvlDesc);
                        availableWeapons.Remove(item);
                        pathsWeapons.Remove(path);
                    }
                    AddItem(item, lvlDesc, path, upgradeOptions[i]);
                }
            }
        }
    }

    /// <summary>
    /// Sets upgrades for the player to choose
    /// </summary>
    public void SetUpgrades(int numOptions)
    {
        numItemsDisplay = numOptions;
        List<ItemDef.LevelDescription> availableDescsAll = new List<ItemDef.LevelDescription>();
        List<Item> availableAll = new List<Item>();
        List<ItemDef.UpgradePath> pathsAll = new List<ItemDef.UpgradePath>();

        // Checks which item upgrades are available to the player
        if (doNewItemsOnNormalLevelUp)
        {
            // Items the player doesn't have
            List<ItemDef> itemDefs = ResourceManager.itemDefs;
            foreach (ItemDef i in itemDefs)
            {
                if (!player.HasItem(i.index))
                {
                    Item item = i.GetItem();
                    if (i.paths.Count > 0)
                    {
                        item.currentPath = i.paths[0];
                        item.Level = 0;
                        availableDescsAll.Add(item.currentPath.levelDescriptions[0]);
                        availableAll.Add(item);
                        pathsAll.Add(item.currentPath);
                    }
                }
            }
        }

        foreach (Item i in player.inventory)
        {
            int offset = 0;
            foreach (ItemDef.UpgradePath u in i.takenPaths)
            {
                offset += u.numLevels;
            }
            if (i.Level < i.itemDef.maxLevel)
            {
                if (i.Level - offset < i.currentPath.numLevels)
                {
                    availableDescsAll.Add(i.currentPath.levelDescriptions[i.Level - offset]);
                    availableAll.Add(i);
                    pathsAll.Add(i.currentPath);
                }
                else
                {
                    foreach (ItemDef.UpgradePath u in i.itemDef.paths)
                    {
                        if (u.previousPath == i.currentPath.pathName)
                        {
                            availableDescsAll.Add(u.levelDescriptions[0]);
                            availableAll.Add(i);
                            pathsAll.Add(u);
                        }
                    }
                }
            }
        }

        // Randomly adds upgrades to the current choices
        int val = Mathf.Min(numOptions, availableAll.Count);
        for (int i = 0; i < val; i++)
        {
            int random = Random.Range(0, availableAll.Count);
            Item item = availableAll[random];
            ItemDef.LevelDescription lvlDesc = availableDescsAll[random];
            ItemDef.UpgradePath path = pathsAll[random];
            availableDescsAll.RemoveAt(random);
            availableAll.RemoveAt(random);
            pathsAll.RemoveAt(random);
            AddItem(item, lvlDesc, path, upgradeOptions[i]);
        }

    }

    /// <summary>
    /// Adds an item to the set of chooseable item upgrades
    /// </summary>
    /// <param name="upgrade"></param>
    public void AddItem(Item item, ItemDef.LevelDescription levelDescription, ItemDef.UpgradePath path, UpgradeOption upgradeOption)
    {
        upgradeOption.gameObject.SetActive(true);
        upgradeOption.SetItem(item.itemDef, item.Level, levelDescription);
        upgradeOption.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        upgradeOption.gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            Select(item, path);
        });
    }

    public void Select(Item item, ItemDef.UpgradePath path)
    {
        item.Level += 1;
        Debug.Log($"Select item: {item.itemDef.itemName} LVL {item.Level}");
        if (item.Level == 1)
        {
            player.inventory.Add(item);
            if (item.itemDef.GetType() == typeof(WeaponDef))
            {
                player.AddWeapon(((WeaponDef)item.itemDef).weaponPrefab);
            }
        }
        if (path != item.currentPath)
        {
            item.takenPaths.Add(item.currentPath);
            item.currentPath = path;
        }
        selected = true;
        chest = null;
    }

    /// <summary>
    /// Resets the panels options
    /// </summary>
    public void Clear()
    {
        foreach (UpgradeOption i in upgradeOptions)
        {
            i.gameObject.SetActive(false);
        }
        selected = false;
        displaying = false;
        baseStatsTxt.text = "";
    }

    public void Reroll()
    {
        player.rerolls--;
        Clear();
        SetUpgrades(numItemsDisplay);
    }

    public void Skip()
    {
        selected = true;
        chest = null;
    }

    /// <summary>
    /// Sets up the panel with the given options
    /// </summary>
    public void ShowOptions()
    {
        if (player.rerolls > 0)
        {
            rerollButton.gameObject.SetActive(true);
            rerollButton.GetComponentInChildren<TMP_Text>().text = "Reroll Upgrades\n" + player.rerolls + " rerolls left";
        }
        else
        {
            rerollButton.gameObject.SetActive(false);
        }

        baseStatsTxt.text =
    "HP: " + player.MaxHp +
    "\nSpeed: " + player.Speed +
    "\nDamage: " + player.Damage +
    "\nAttack Speed: " + player.AttackSpeed * 100 + "%" +
    "\nArmor: " + player.Armor +
    "\nRegen: " + player.Regen +
    "\nCrit Chance: " + (player.CritChance * 100) + "%" +
    "\nCrit Damage: " + player.CritDamage * 100 + "%";

        displaying = true;
    }

    /*
    /// <summary>
    /// Called when the player hits the accept upgrade button
    /// </summary>
    /// <param name="upgrade"></param>
    public void Select(ResourceManager.UpgradeIndex upgrade)
    {
        if (ResourceManager.GetUpgrade(upgrade).IsWeapon && player.weapons.Count + tempWeapons >= GameManager.instance.weaponIcons.Count)
        {
            replaceWeaponButton.gameObject.SetActive(false);
            rerollButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);

            foreach (UpgradeOption i in options)
            {
                i.gameObject.SetActive(false);
            }
            textName.text = "";
            tier.text = "";
            desc.text = "";
            titleText.text = "Replace a weapon";

            // Adding replace weapon options
            for (int i = 0; i < player.weapons.Count; i++)
            {
                UpgradeOption option = Instantiate<UpgradeOption>(prefab, new Vector3(), new Quaternion(), infoPanel.gameObject.transform);
                UpgradeIndex replacement = player.weapons[i].upgradeIndex;
                option.upgrade = replacement;
                option.tier = tier;
                option.textName = textName;
                option.desc = desc;
                RectTransform upgradeRect = option.GetComponent<RectTransform>();
                upgradeRect.localScale = new Vector3(2, 2, 1);
                option.isWeaponReplacement = true;
                option.baseStatsTxt = baseStatsTxt;
                option.weaponStatsTxt = weaponStatsTxt;
                option.weaponStatsLabel = weaponStatsLabel;
                option.gameObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    //SelectWeapon(upgrade, replacement);
                });
                replaceWeapons.Add(option);
            }

            for (int i = 0; i < replaceWeapons.Count; i++)
            {
                UpgradeOption option = replaceWeapons[i];

                RectTransform upgradeRect = option.GetComponent<RectTransform>();

                upgradeRect.localPosition = new Vector3(0, 0);
                upgradeRect.anchorMax = new Vector2(0.125f + (i * 0.25f), 0.3f);
                upgradeRect.anchorMin = new Vector2(0.125f + (i * 0.25f), 0.3f);
            }
        }
        else
        {
            if (ResourceManager.GetUpgrade(upgrade).IsWeapon)
            {
                tempWeapons++;
            }
            //player.AddUpgrade(upgrade);
            selected = true;
        }
    }

    /// <summary>
    /// Sets up the panel with the given options
    /// </summary>
    public void ShowOptions(int levels, bool isWeapons)
    {
        this.levels = levels;
        if (isWeapons)
        {
            rerollButton.gameObject.SetActive(false);
            replaceWeaponButton.gameObject.SetActive(true);
        }
        else if (player.rerolls > 0)
        {
            rerollButton.gameObject.SetActive(true);
            rerollButton.GetComponentInChildren<TMP_Text>().text = "Reroll Upgrades\n" + player.rerolls + " rerolls left";
            replaceWeaponButton.gameObject.SetActive(false);
        }
        else
        {
            rerollButton.gameObject.SetActive(false);
            replaceWeaponButton.gameObject.SetActive(false);
        }
        if (levels == 1)
        {
            titleText.text = "Select <b><color=#00D4FF>" + levels + "</color></b> Upgrade";
        }
        else
        {
            titleText.text = "Select <b><color=#00D4FF>" + levels + "</color></b> Upgrades";
        }

        textName.text = "";
        tier.text = "";
        desc.text = "";
        for (int i = 0; i < options.Count; i++)
        {
            UpgradeOption option = options[i];

            RectTransform upgradeRect = option.GetComponent<RectTransform>();

            upgradeRect.localPosition = new Vector3(0, 0);
            upgradeRect.anchorMax = new Vector2(0.125f + (i * 0.25f), 0.3f);
            upgradeRect.anchorMin = new Vector2(0.125f + (i * 0.25f), 0.3f);
        }
        displaying = true;
    }

    /// <summary>
    /// Adds an upgrade to the set of chooseable ones
    /// </summary>
    /// <param name="upgrade"></param>
    public void AddUpgrade(UpgradeIndex upgrade)
    {
        UpgradeOption option = Instantiate<UpgradeOption>(prefab, new Vector3(), new Quaternion(), infoPanel.gameObject.transform);
        option.upgrade = upgrade;
        option.tier = tier;
        option.textName = textName;
        option.desc = desc;
        option.baseStatsTxt = baseStatsTxt;
        option.weaponStatsTxt = weaponStatsTxt;
        option.weaponStatsLabel = weaponStatsLabel;
        RectTransform upgradeRect = option.GetComponent<RectTransform>();
        upgradeRect.localScale = new Vector3(2, 2, 1);
        option.gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            Select(upgrade);
        });
        options.Add(option);
    }

    /// <summary>
    /// Sets upgrades for the player to choose
    /// </summary>
    public void SetUpgradesByPool(UpgradePool pool, int numOptions)
    {
        List<ResourceManager.UpgradeIndex> commons = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> uncommons = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> rares = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> epics = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> legendaries = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> all = new List<ResourceManager.UpgradeIndex>();
        foreach (ResourceManager.UpgradeIndex u in pool.upgrades)
        {
            /*
            if (!(player.HasUpgrade(u) && !ResourceManager.GetUpgrade(u).CanTakeMultiple))
            {
                Upgrade upgrade = ResourceManager.GetUpgrade(u);
                if (upgrade.tier == Upgrade.Tier.Common)
                {
                    commons.Add(u);
                }
                else if (upgrade.tier == Upgrade.Tier.Uncommon)
                {
                    uncommons.Add(u);
                }
                else if (upgrade.tier == Upgrade.Tier.Rare)
                {
                    rares.Add(u);
                }
                else if (upgrade.tier == Upgrade.Tier.Epic)
                {
                    epics.Add(u);
                }
                else if (upgrade.tier == Upgrade.Tier.Legendary)
                {
                    legendaries.Add(u);
                }
                all.Add(u);
            }
        }
        for (int i = 0; i < numOptions; i++)
        {
            float roll = Random.value;
            if (roll <= legendaryOdds && legendaries.Count > 0)
            {
                int random = Random.Range(0, legendaries.Count);
                ResourceManager.UpgradeIndex upgrade = legendaries[random];
                legendaries.RemoveAt(random);
                all.Remove(upgrade);
                AddUpgrade(upgrade);
            }
            else if (roll <= epicOdds && epics.Count > 0)
            {
                int random = Random.Range(0, epics.Count);
                ResourceManager.UpgradeIndex upgrade = epics[random];
                epics.RemoveAt(random);
                all.Remove(upgrade);
                AddUpgrade(upgrade);
            }
            else if (roll <= rareOdds && rares.Count > 0)
            {
                int random = Random.Range(0, rares.Count);
                ResourceManager.UpgradeIndex upgrade = rares[random];
                rares.RemoveAt(random);
                all.Remove(upgrade);
                AddUpgrade(upgrade);
            }
            else if (roll <= uncommonOdds && uncommons.Count > 0)
            {
                int random = Random.Range(0, uncommons.Count);
                ResourceManager.UpgradeIndex upgrade = uncommons[random];
                uncommons.RemoveAt(random);
                all.Remove(upgrade);
                AddUpgrade(upgrade);
            }
            else if (roll <= commonOdds && commons.Count > 0)
            {
                int random = Random.Range(0, commons.Count);
                ResourceManager.UpgradeIndex upgrade = commons[random];
                commons.RemoveAt(random);
                all.Remove(upgrade);
                AddUpgrade(upgrade);
            }
            else if (all.Count > 0)
            {
                int random = Random.Range(0, all.Count);
                ResourceManager.UpgradeIndex upgrade = all[random];
                all.RemoveAt(random);
                AddUpgrade(upgrade);
            }
        }
    }

    /// <summary>
    /// Sets upgrades for the player to choose from the given pools
    /// </summary>
    public void SetUpgradesByPools(List<UpgradePool> pools, int numOptions)
    {
        List<ResourceManager.UpgradeIndex> commons = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> uncommons = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> rares = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> epics = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> legendaries = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> all = new List<ResourceManager.UpgradeIndex>();
        foreach (UpgradePool pool in pools)
        {
            foreach (ResourceManager.UpgradeIndex u in pool.upgrades)
            {
                /*
                if (!(player.HasUpgrade(u) && !ResourceManager.GetUpgrade(u).CanTakeMultiple))
                {
                    Upgrade upgrade = ResourceManager.GetUpgrade(u);
                    if (upgrade.tier == Upgrade.Tier.Common)
                    {
                        commons.Add(u);
                    }
                    else if (upgrade.tier == Upgrade.Tier.Uncommon)
                    {
                        uncommons.Add(u);
                    }
                    else if (upgrade.tier == Upgrade.Tier.Rare)
                    {
                        rares.Add(u);
                    }
                    else if (upgrade.tier == Upgrade.Tier.Epic)
                    {
                        epics.Add(u);
                    }
                    else if (upgrade.tier == Upgrade.Tier.Legendary)
                    {
                        legendaries.Add(u);
                    }
                    all.Add(u);
                }
            }
        }
        for (int i = 0; i < numOptions; i++)
        {
            float roll = Random.value;
            if (roll <= legendaryOdds && legendaries.Count > 0)
            {
                int random = Random.Range(0, legendaries.Count);
                ResourceManager.UpgradeIndex upgrade = legendaries[random];
                legendaries.RemoveAt(random);
                all.Remove(upgrade);
                AddUpgrade(upgrade);
            }
            else if (roll <= epicOdds && epics.Count > 0)
            {
                int random = Random.Range(0, epics.Count);
                ResourceManager.UpgradeIndex upgrade = epics[random];
                epics.RemoveAt(random);
                all.Remove(upgrade);
                AddUpgrade(upgrade);
            }
            else if (roll <= rareOdds && rares.Count > 0)
            {
                int random = Random.Range(0, rares.Count);
                ResourceManager.UpgradeIndex upgrade = rares[random];
                rares.RemoveAt(random);
                all.Remove(upgrade);
                AddUpgrade(upgrade);
            }
            else if (roll <= uncommonOdds && uncommons.Count > 0)
            {
                int random = Random.Range(0, uncommons.Count);
                ResourceManager.UpgradeIndex upgrade = uncommons[random];
                uncommons.RemoveAt(random);
                all.Remove(upgrade);
                AddUpgrade(upgrade);
            }
            else if (roll <= commonOdds && commons.Count > 0)
            {
                int random = Random.Range(0, commons.Count);
                ResourceManager.UpgradeIndex upgrade = commons[random];
                commons.RemoveAt(random);
                all.Remove(upgrade);
                AddUpgrade(upgrade);
            }
            else if (all.Count > 0)
            {
                int random = Random.Range(0, all.Count);
                ResourceManager.UpgradeIndex upgrade = all[random];
                all.RemoveAt(random);
                AddUpgrade(upgrade);
            }
        }
    }

    /// <summary>
    /// Resets the panels options
    /// </summary>
    public void Clear()
    {
        for (int i = options.Count - 1; i >= 0; i--)
        {
            Destroy(options[i].gameObject);
        }
        for (int i = replaceWeapons.Count - 1; i >= 0; i--)
        {
            Destroy(replaceWeapons[i].gameObject);
        }
        titleText.text = "Select an Upgrade";
        options.Clear();
        replaceWeapons.Clear();
        selected = false;
        displaying = false;
        baseStatsTxt.text = "";
        weaponStatsLabel.text = "";
        weaponStatsTxt.text = "";
    }

    private void ResetOdds()
    {
        float totalWeight = commonOdds + uncommonOdds + rareOdds + epicOdds + legendaryOdds;
        legendaryOdds = legendaryOdds / totalWeight;
        epicOdds = epicOdds / totalWeight;
        rareOdds = rareOdds / totalWeight;
        uncommonOdds = uncommonOdds / totalWeight;
        commonOdds = commonOdds / totalWeight;

        epicOdds += legendaryOdds;
        rareOdds += epicOdds;
        uncommonOdds += rareOdds;
        commonOdds += uncommonOdds;
    }

    public void Reroll()
    {
        player.rerolls--;
        Clear();
        SetUpgradesByPools(GameManager.instance.GetPossiblePools(true), 4);
        ShowOptions(levels, false);
    }

    public void ReplaceWeapons()
    {
        Clear();
        SetUpgradesByPools(GameManager.instance.GetPossiblePools(true), 4);
        ShowOptions(levels, false);
    }

    public void GoBack()
    {
        for (int i = replaceWeapons.Count - 1; i >= 0; i--)
        {
            Destroy(replaceWeapons[i].gameObject);
        }
        replaceWeapons.Clear();

        foreach (UpgradeOption i in options)
        {
            i.gameObject.SetActive(true);
        }

        backButton.gameObject.SetActive(false);
        replaceWeaponButton.gameObject.SetActive(true);
        textName.text = "";
        tier.text = "";
        desc.text = "";
        if (levels == 1)
        {
            titleText.text = "Select <b><color=#00D4FF>" + levels + "</color></b> Upgrade";
        }
        else
        {
            titleText.text = "Select <b><color=#00D4FF>" + levels + "</color></b> Upgrades";
        }
    }
    */
}
