using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelManager : MonoBehaviour
{
    private List<UpgradeOption> options = new List<UpgradeOption>();
    public UpgradeOption optionPrefab;
    public Player player;
    public bool selected = false;
    public bool displaying = false;
    [Range(0, 1)]
    public float commonOdds, uncommonOdds, rareOdds;

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
        for (int i = 0; i < options.Count; i++)
        {
            UpgradeOption option = options[i];

            RectTransform rect = option.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 0);
            rect.anchoredPosition = new Vector2(0, 0);
            rect.anchorMin = new Vector2(((float)i / (float)options.Count) + 0.01f, 0.2f);
            rect.anchorMax = new Vector2(((float)(i + 1) / (float)options.Count) - 0.01f, 0.8f);
        }
        displaying = true;
    }

    /// <summary>
    /// Adds an upgrade to the set of chooseable ones
    /// </summary>
    /// <param name="upgrade"></param>
    public void AddUpgrade(ResourceManager.UpgradeIndex upgrade)
    {
        UpgradeOption option = Instantiate<UpgradeOption>(optionPrefab, new Vector3(), new Quaternion(), this.gameObject.transform);
        option.manager = this;
        option.SetUpgrade(upgrade);
        options.Add(option);
    }

    /// <summary>
    /// Sets upgrades for the player to choose
    /// </summary>
    public void SetUpgradesByPool(UpgradePool pool, int numOptions)
    {
        ResetOdds();
        List<ResourceManager.UpgradeIndex> commons = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> uncommons = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> rares = new List<ResourceManager.UpgradeIndex>();
        foreach (ResourceManager.UpgradeIndex u in pool.upgrades)
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
        }
        for (int i = 0; i < numOptions; i++)
        {
            float roll = Random.value;
            if (roll <= commonOdds)
            {
                int random = Random.Range(0, commons.Count);
                ResourceManager.UpgradeIndex upgrade = commons[random];
                AddUpgrade(upgrade);
            }
            else if (roll - commonOdds <= uncommonOdds)
            {
                int random = Random.Range(0, uncommons.Count);
                ResourceManager.UpgradeIndex upgrade = uncommons[random];
                AddUpgrade(upgrade);
            }
            else
            {
                int random = Random.Range(0, rares.Count);
                ResourceManager.UpgradeIndex upgrade = rares[random];
                AddUpgrade(upgrade);
            }
        }
    }

    /// <summary>
    /// Sets upgrades for the player to choose from the given pools
    /// </summary>
    public void SetUpgradesByPools(List<UpgradePool> pools, int numOptions)
    {
        ResetOdds();
        List<ResourceManager.UpgradeIndex> commons = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> uncommons = new List<ResourceManager.UpgradeIndex>();
        List<ResourceManager.UpgradeIndex> rares = new List<ResourceManager.UpgradeIndex>();
        foreach (UpgradePool pool in pools)
        {
            foreach (ResourceManager.UpgradeIndex u in pool.upgrades)
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
            }
        }
        for (int i = 0; i < numOptions; i++)
        {
            float roll = Random.value;
            if (roll <= commonOdds)
            {
                int random = Random.Range(0, commons.Count);
                ResourceManager.UpgradeIndex upgrade = commons[random];
                AddUpgrade(upgrade);
            }
            else if (roll - commonOdds <= uncommonOdds)
            {
                int random = Random.Range(0, uncommons.Count);
                ResourceManager.UpgradeIndex upgrade = uncommons[random];
                AddUpgrade(upgrade);
            }
            else
            {
                int random = Random.Range(0, rares.Count);
                ResourceManager.UpgradeIndex upgrade = rares[random];
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
        float totalWeight = commonOdds + uncommonOdds + rareOdds;
        commonOdds = commonOdds / totalWeight;
        uncommonOdds = uncommonOdds / totalWeight;
        rareOdds = rareOdds / totalWeight;
    }
}
