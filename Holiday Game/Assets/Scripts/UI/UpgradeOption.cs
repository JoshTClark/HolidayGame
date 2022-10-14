using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeOption : MonoBehaviour
{
    [SerializeField]
    private TMP_Text nameObject, desriptionObject;

    [SerializeField]
    private Button button;

    public ResourceManager.UpgradeIndex upgrade;

    public UpgradePanelManager manager;

    private void Start()
    {
    }

    public void OnButtonClick()
    {
        manager.Select(upgrade);
    }

    public void SetUpgrade(ResourceManager.UpgradeIndex u)
    {
        upgrade = u;
        Upgrade upgradeObject = ResourceManager.GetUpgrade(u);
        nameObject.text = upgradeObject.upgradeName;
        desriptionObject.text = upgradeObject.upgradeDescription;

        if (upgradeObject.tier == Upgrade.Tier.Common)
        {
            this.gameObject.GetComponent<Image>().color = new Color(0f, 1f, 0f, 0.5f);
        } else if (upgradeObject.tier == Upgrade.Tier.Uncommon)
        {
            this.gameObject.GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.5f);
        }else if (upgradeObject.tier == Upgrade.Tier.Rare)
        {
            this.gameObject.GetComponent<Image>().color = new Color(1f, 0f, 1f, 0.5f);
        }
    }
}
