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

    public Upgrade upgrade;

    public UpgradePanelManager manager;

    public void OnButtonClick()
    {
        
    }
}
