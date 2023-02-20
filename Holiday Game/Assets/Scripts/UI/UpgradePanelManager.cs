using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ResourceManager;

public class UpgradePanelManager : MonoBehaviour
{
    private List<UpgradeOption> options = new List<UpgradeOption>();
    public UpgradeOption prefab;
    public Player player;
    public bool selected = false;
    public bool displaying = false;
    public float commonOdds, uncommonOdds, rareOdds, epicOdds, legendaryOdds;
    public TMP_Text textName, tier, desc, titleText, baseStatsTxt, weaponStatsTxt, weaponStatsLabel;
    public Button rerollButton, backButton;
    private int levels = 1;
    public CanvasRenderer infoPanel, statsPanel;

    private void Start()
    {
        rerollButton.gameObject.SetActive(false);
    }
    /// <summary>
    /// Sets upgrades for the player to choose
    /// </summary>
    public void SetUpgrades(int numOptions, bool specialUpgrade, bool itemsOnly, bool weaponsOnly)
    {
        List<ItemDef.LevelDescription> availableDescs = new List<ItemDef.LevelDescription>();
        List<Item> available = new List<Item>();
        List<ItemDef.UpgradePath> paths = new List<ItemDef.UpgradePath>();

        if (specialUpgrade)
        {
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
                        availableDescs.Add(item.currentPath.levelDescriptions[0]);
                        available.Add(item);
                        paths.Add(item.currentPath);
                    }
                }
            }
        }
        else
        {
            // Checks which item upgrades are available to the player
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
                        availableDescs.Add(i.currentPath.levelDescriptions[i.Level - offset]);
                        available.Add(i);
                        paths.Add(i.currentPath);
                    }
                    else
                    {
                        foreach (ItemDef.UpgradePath u in i.itemDef.paths)
                        {
                            if (u.previousPath == i.currentPath.pathName)
                            {
                                availableDescs.Add(u.levelDescriptions[0]);
                                available.Add(i);
                                paths.Add(u);
                            }
                        }
                    }
                }
            }
        }

        // Randomly adds upgrades to the current choices
        int val = Mathf.Min(numOptions, available.Count);
        for (int i = 0; i < val; i++)
        {
            int random = Random.Range(0, available.Count);
            Item item = available[random];
            ItemDef.LevelDescription lvlDesc = availableDescs[random];
            ItemDef.UpgradePath path = paths[random];
            availableDescs.RemoveAt(random);
            available.RemoveAt(random);
            paths.RemoveAt(random);
            AddItem(item, lvlDesc, path);
        }
    }

    public void SetUpgrades(int numOptions)
    {
        SetUpgrades(numOptions, false, false, false);
    }

    /// <summary>
    /// Adds an item to the set of chooseable item upgrades
    /// </summary>
    /// <param name="upgrade"></param>
    public void AddItem(Item item, ItemDef.LevelDescription levelDescription, ItemDef.UpgradePath path)
    {
        UpgradeOption option = Instantiate<UpgradeOption>(prefab, new Vector3(), new Quaternion(), infoPanel.gameObject.transform);
        option.item = item;
        option.levelDescription = levelDescription;
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
            Select(item, path);
        });
        options.Add(option);
    }

    public void Select(Item item, ItemDef.UpgradePath path)
    {
        item.Level += 1;
        if (item.Level == 1)
        {
            player.inventory.Add(item);
        }
        if (path != item.currentPath)
        {
            item.takenPaths.Add(item.currentPath);
            item.currentPath = path;
        }
        selected = true;
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
        titleText.text = "Select an Upgrade";
        options.Clear();
        selected = false;
        displaying = false;
        baseStatsTxt.text = "";
        weaponStatsLabel.text = "";
        weaponStatsTxt.text = "";
    }

    public void Reroll()
    {
        player.rerolls--;
        Clear();
        SetUpgrades(4);
    }

    public void Skip()
    {
        selected = true;
    }

    /// <summary>
    /// Sets up the panel with the given options
    /// </summary>
    public void ShowOptions(int levels)
    {
        this.levels = levels;
        if (player.rerolls > 0)
        {
            rerollButton.gameObject.SetActive(true);
            rerollButton.GetComponentInChildren<TMP_Text>().text = "Reroll Upgrades\n" + player.rerolls + " rerolls left";
        }
        else
        {
            rerollButton.gameObject.SetActive(false);
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
