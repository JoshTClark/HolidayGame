using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaUpgrade : MonoBehaviour
{
    [SerializeField]
    private int cost;

    public bool selected;
    public int level;
    private static MetaUpgrade prev;
    public string upgradeName;
    public string upgradeDesc;
    public TMP_Text levelText;

    private void Update()
    {
        if (selected)
        {
            GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.75f);
        }
        else
        {
            GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.25f);
        }

        levelText.text = upgradeName + " " + level;
    }

    public void Select()
    {
        if (prev)
        {
            prev.selected = false;
        }
        selected = true;
        prev = this;
    }

    public int GetCost()
    {
        return cost;
    }
}
