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
    }
}
