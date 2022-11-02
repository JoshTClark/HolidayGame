using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeOption : MonoBehaviour
{
    public ResourceManager.UpgradeIndex upgrade;
    public TMP_Text textName, tier, desc;
    public Image iconHolder;
    public bool isHover = false;
    public bool isWeaponReplacement = false;
    private float scale = 2;

    private void Start()
    {
        Upgrade upgradeObject = ResourceManager.GetUpgrade(upgrade);

        // Setting the icon
        if (ResourceManager.GetUpgrade(upgrade).icon)
        {
            iconHolder.sprite = ResourceManager.GetUpgrade(upgrade).icon;
        }

        // Setting the color
        if (upgradeObject.tier == Upgrade.Tier.Common)
        {
            this.gameObject.GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);
        }
        else if (upgradeObject.tier == Upgrade.Tier.Uncommon)
        {
            this.gameObject.GetComponent<Image>().color = new Color(0f, 0f, 1f, 1f);
        }
        else if (upgradeObject.tier == Upgrade.Tier.Rare)
        {
            this.gameObject.GetComponent<Image>().color = new Color(1f, 0f, 1f, 1f);
        }
        else if (upgradeObject.tier == Upgrade.Tier.Epic)
        {
            this.gameObject.GetComponent<Image>().color = new Color(1f, 0.5f, 0f, 1f);
        }
        else if (upgradeObject.tier == Upgrade.Tier.Legendary)
        {
            this.gameObject.GetComponent<Image>().color = new Color(0.8f, 0f, 0f, 1f);
        }

    }

    public void Update()
    {
        float delta = Time.deltaTime;

        // Hover animation
        RectTransform rectTransform = this.gameObject.GetComponent<RectTransform>();
        if (isHover && scale < 2.5f)
        {
            scale += 5f * delta;
            if (scale > 2.5f)
            {
                scale = 2.5f;
            }
        }
        else if (!isHover && scale > 2f)
        {
            scale -= 5f * delta;
            if (scale < 2f)
            {
                scale = 2f;
            }
        }

        rectTransform.localScale = new Vector3(scale, scale, 1);
    }

    public void OnHoverStart()
    {
        isHover = true;
        Upgrade upgradeObject = ResourceManager.GetUpgrade(upgrade);
        if (isWeaponReplacement)
        {
            textName.text = upgradeObject.upgradeName;
            desc.text = "Replace this weapon?";
        }
        else
        {
            textName.text = upgradeObject.upgradeName;
            desc.text = upgradeObject.upgradeDescription;
            tier.enableVertexGradient = false;
            textName.enableVertexGradient = false;
            if (upgradeObject.tier == Upgrade.Tier.Common)
            {
                tier.text = "Common";
                tier.color = new Color(0f, 1f, 0f, 1f);
                textName.color = new Color(0f, 1f, 0f, 1f);
            }
            else if (upgradeObject.tier == Upgrade.Tier.Uncommon)
            {
                tier.text = "Uncommon";
                tier.color = new Color(0f, 0f, 1f, 1f);
                textName.color = new Color(0f, 0f, 1f, 1f);
            }
            else if (upgradeObject.tier == Upgrade.Tier.Rare)
            {
                tier.text = "Rare";
                tier.color = new Color(1f, 0f, 1f, 1f);
                textName.color = new Color(1f, 0f, 1f, 1f);
            }
            else if (upgradeObject.tier == Upgrade.Tier.Epic)
            {
                VertexGradient grad = new VertexGradient();
                grad.topLeft = new Color(1f, 0.5f, 0f, 1f);
                grad.bottomLeft = new Color(0.75f, 0.35f, 0f, 1f);
                grad.topRight = new Color(0.75f, 0.35f, 0f, 1f);
                grad.bottomRight = new Color(0.5f, 0.25f, 0f, 1f);
                tier.enableVertexGradient = true;
                textName.enableVertexGradient = true;
                tier.color = new Color(1f, 1f, 1f, 1f);
                textName.color = new Color(1f, 1f, 1f, 1f);
                tier.text = "Epic";
                tier.colorGradient = grad;
                textName.colorGradient = grad;
            }
            else if (upgradeObject.tier == Upgrade.Tier.Legendary)
            {
                VertexGradient grad = new VertexGradient();
                grad.topLeft = new Color(1f, 0f, 0f, 1f);
                grad.bottomLeft = new Color(0.75f, 0f, 0f, 1f);
                grad.topRight = new Color(0.75f, 0f, 0f, 1f);
                grad.bottomRight = new Color(0.5f, 0f, 0f, 1f);
                tier.enableVertexGradient = true;
                textName.enableVertexGradient = true;
                tier.text = "Legendary";
                tier.color = new Color(1f, 1f, 1f, 1f);
                textName.color = new Color(1f, 1f, 1f, 1f);
                tier.colorGradient = grad;
                textName.colorGradient = grad;
            }

            if (upgradeObject.IsWeapon)
            {
                tier.text = "New Weapon";
            }
        }
    }

    public void OnHoverStop()
    {
        isHover = false;
    }
}
