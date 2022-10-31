using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelManager : MonoBehaviour
{
    private List<UpgradeOption> options = new List<UpgradeOption>();
    public UpgradeOption prefab;
    public Player player;
    public bool selected = false;
    public bool displaying = false;
    public float commonOdds, uncommonOdds, rareOdds, epicOdds, legendaryOdds;
    public TMP_Text textName, tier, desc;

    private void Start()
    {
        ResetOdds();
    }

    /// <summary>
    /// Called when the player hits the accept upgrade button
    /// </summary>
    /// <param name="upgrade"></param>
    public void Select(ResourceManager.UpgradeIndex upgrade)
    {
        player.AddUpgrade(upgrade);
        selected = true;
    }

    /// <summary>
    /// Sets up the panel with the given options
    /// </summary>
    public void ShowOptions()
    {
        textName.text = "";
        tier.text = "";
        desc.text = "";
        for (int i = 0; i < options.Count; i++)
        {
            UpgradeOption option = options[i];

            RectTransform upgradeRect = option.GetComponent<RectTransform>();
            RectTransform panelRect = this.gameObject.GetComponent<RectTransform>();

            upgradeRect.SetPositionAndRotation(new Vector3(((panelRect.rect.width / (options.Count + 1)) * (i + 1)) * panelRect.lossyScale.x, (panelRect.rect.height / 4) * panelRect.lossyScale.y, 0), Quaternion.identity);
        }
        displaying = true;
    }

    /// <summary>
    /// Adds an upgrade to the set of chooseable ones
    /// </summary>
    /// <param name="upgrade"></param>
    public void AddUpgrade(ResourceManager.UpgradeIndex upgrade)
    {
        UpgradeOption option = Instantiate<UpgradeOption>(prefab, new Vector3(), new Quaternion(), this.gameObject.transform);
        option.upgrade = upgrade;
        option.tier = tier;
        option.textName = textName;
        option.desc = desc;
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
        foreach (ResourceManager.UpgradeIndex u in pool.upgrades)
        {
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
                AddUpgrade(upgrade);
            }
            else if (roll <= epicOdds && epics.Count > 0)
            {
                int random = Random.Range(0, epics.Count);
                ResourceManager.UpgradeIndex upgrade = epics[random];
                epics.RemoveAt(random);
                AddUpgrade(upgrade);
            }
            else if (roll <= rareOdds && rares.Count > 0)
            {
                int random = Random.Range(0, rares.Count);
                ResourceManager.UpgradeIndex upgrade = rares[random];
                rares.RemoveAt(random);
                AddUpgrade(upgrade);
            }
            else if (roll <= uncommonOdds && uncommons.Count > 0)
            {
                int random = Random.Range(0, uncommons.Count);
                ResourceManager.UpgradeIndex upgrade = uncommons[random];
                uncommons.RemoveAt(random);
                AddUpgrade(upgrade);
            }
            else if (roll <= commonOdds && commons.Count > 0)
            {
                int random = Random.Range(0, commons.Count);
                ResourceManager.UpgradeIndex upgrade = commons[random];
                commons.RemoveAt(random);
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
        foreach (UpgradePool pool in pools)
        {
            foreach (ResourceManager.UpgradeIndex u in pool.upgrades)
            {
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
                AddUpgrade(upgrade);
            }
            else if (roll <= epicOdds && epics.Count > 0)
            {
                int random = Random.Range(0, epics.Count);
                ResourceManager.UpgradeIndex upgrade = epics[random];
                epics.RemoveAt(random);
                AddUpgrade(upgrade);
            }
            else if (roll <= rareOdds && rares.Count > 0)
            {
                int random = Random.Range(0, rares.Count);
                ResourceManager.UpgradeIndex upgrade = rares[random];
                rares.RemoveAt(random);
                AddUpgrade(upgrade);
            }
            else if (roll <= uncommonOdds && uncommons.Count > 0)
            {
                int random = Random.Range(0, uncommons.Count);
                ResourceManager.UpgradeIndex upgrade = uncommons[random];
                uncommons.RemoveAt(random);
                AddUpgrade(upgrade);
            }
            else if (roll <= commonOdds && commons.Count > 0)
            {
                int random = Random.Range(0, commons.Count);
                ResourceManager.UpgradeIndex upgrade = commons[random];
                commons.RemoveAt(random);
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
        options.Clear();
        selected = false;
        displaying = false;
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
}
