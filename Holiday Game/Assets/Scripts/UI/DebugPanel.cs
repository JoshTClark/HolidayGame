using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ResourceManager;

public class DebugPanel : MonoBehaviour
{
    public TMP_Dropdown upgradeSelector;
    public TMP_Text inventoryDisplay;

    public void Init()
    {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (Upgrade i in ResourceManager.upgradeDefinitions)
        {
            options.Add(new TMP_Dropdown.OptionData(i.upgradeName));
        }
        upgradeSelector.AddOptions(options);
    }

    private void Update()
    {
        inventoryDisplay.text = "";
        foreach (Upgrade i in GameManager.instance.Player.inventory)
        {
            inventoryDisplay.text += i.upgradeName + ": LVL " + i.CurrentLevel + "\n";
        }
    }

    public void ButtonPressed()
    {
        UpgradeIndex index = ResourceManager.UpgradeIndexFromName(upgradeSelector.options[upgradeSelector.value].text);
        Debug.Log("Giving player " + ResourceManager.GetUpgrade(index).upgradeName);
        GameManager.instance.Player.AddUpgrade(index);
    }
}
