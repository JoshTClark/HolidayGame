using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeOption : MonoBehaviour
{
    public Item item;
    public ItemDef.LevelDescription levelDescription;
    public TMP_Text textName, tier, desc, baseStatsTxt, weaponStatsTxt, weaponStatsLabel;
    public Image iconHolder;
    public bool isHover = false;
    private float scale = 2;

    private void Start()
    {
        // Setting the icon
        if (item.itemDef.icon)
        {
            iconHolder.sprite = item.itemDef.icon;
        }

        // Setting the color
        if (item.itemDef.tier == ItemDef.Tier.Common)
        {
            this.gameObject.GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);
        }
        else if (item.itemDef.tier == ItemDef.Tier.Uncommon)
        {
            this.gameObject.GetComponent<Image>().color = new Color(0f, 0f, 1f, 1f);
        }
        else if (item.itemDef.tier == ItemDef.Tier.Rare)
        {
            this.gameObject.GetComponent<Image>().color = new Color(1f, 0f, 1f, 1f);
        }
        else if (item.itemDef.tier == ItemDef.Tier.Epic)
        {
            this.gameObject.GetComponent<Image>().color = new Color(1f, 0.5f, 0f, 1f);
        }
        else if (item.itemDef.tier == ItemDef.Tier.Legendary)
        {
            this.gameObject.GetComponent<Image>().color = new Color(0.8f, 0f, 0f, 1f);
        }

    }

    public void Update()
    {
        float delta = Time.unscaledDeltaTime;

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

    /// <summary>
    /// Called when player hovers over the button
    /// Sets the stat change descriptions
    /// </summary>
    public void OnHoverStart()
    {
        isHover = true;
        textName.text = levelDescription.lvlName;
        desc.text = levelDescription.desc;
        tier.text = item.itemDef.itemName + " LVL: " + (item.Level + 1);
        tier.enableVertexGradient = false;
        textName.enableVertexGradient = false;
        if (item.itemDef.tier == ItemDef.Tier.Common)
        {
            //tier.text = "Common";
            tier.color = new Color(0f, 1f, 0f, 1f);
            textName.color = new Color(0f, 1f, 0f, 1f);
        }
        else if (item.itemDef.tier == ItemDef.Tier.Uncommon)
        {
            //tier.text = "Uncommon";
            tier.color = new Color(0f, 0f, 1f, 1f);
            textName.color = new Color(0f, 0f, 1f, 1f);
        }
        else if (item.itemDef.tier == ItemDef.Tier.Rare)
        {
            //tier.text = "Rare";
            tier.color = new Color(1f, 0f, 1f, 1f);
            textName.color = new Color(1f, 0f, 1f, 1f);
        }
        else if (item.itemDef.tier == ItemDef.Tier.Epic)
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
            //tier.text = "Epic";
            tier.colorGradient = grad;
            textName.colorGradient = grad;
        }
        else if (item.itemDef.tier == ItemDef.Tier.Legendary)
        {
            VertexGradient grad = new VertexGradient();
            grad.topLeft = new Color(1f, 0f, 0f, 1f);
            grad.bottomLeft = new Color(0.75f, 0f, 0f, 1f);
            grad.topRight = new Color(0.75f, 0f, 0f, 1f);
            grad.bottomRight = new Color(0.5f, 0f, 0f, 1f);
            tier.enableVertexGradient = true;
            textName.enableVertexGradient = true;
            //tier.text = "Legendary";
            tier.color = new Color(1f, 1f, 1f, 1f);
            textName.color = new Color(1f, 1f, 1f, 1f);
            tier.colorGradient = grad;
            textName.colorGradient = grad;
        }


        Player player = GameManager.instance.Player;

        float newHPf = (player.BaseMaxHp + levelDescription.statChanges.hpAdd + player.hpAdd) * player.hpMult * levelDescription.statChanges.hpMult;
        string newHP;
        if (newHPf > player.MaxHp)
        {
            newHP = "<color=#27FF00>" + newHPf + "</color>";
        }
        else if (newHPf < player.MaxHp)
        {
            newHP = "<color=#BD0000>" + newHPf + "</color>";
        }
        else
        {
            newHP = newHPf.ToString();
        }

        float newSpeedf = (player.BaseSpeed + levelDescription.statChanges.speedAdd + player.speedAdd) * player.speedMult * levelDescription.statChanges.speedMult;
        string newSpeed;
        if (newSpeedf > player.Speed)
        {
            newSpeed = "<color=#27FF00>" + newSpeedf + "</color>";
        }
        else if (newSpeedf < player.Speed)
        {
            newSpeed = "<color=#BD0000>" + newSpeedf + "</color>";
        }
        else
        {
            newSpeed = newSpeedf.ToString();
        }

        float newDamagef = (player.BaseDamage + levelDescription.statChanges.damageAdd + player.damageAdd) * player.damageMult * levelDescription.statChanges.damageMult;
        string newDamage;
        if (newDamagef > player.Damage)
        {
            newDamage = "<color=#27FF00>" + newDamagef + "</color>";
        }
        else if (newDamagef < player.Damage)
        {
            newDamage = "<color=#BD0000>" + newDamagef + "</color>";
        }
        else
        {
            newDamage = newDamagef.ToString();
        }

        float newAtckSpdf = (player.BaseAttackSpeed + levelDescription.statChanges.attackSpeedAdd + player.attackSpeedAdd) * player.attackSpeedMult * levelDescription.statChanges.attackSpeedMult;
        string newAtckSpd;
        if (newAtckSpdf > player.AttackSpeed)
        {
            newAtckSpd = "<color=#27FF00>" + newAtckSpdf * 100 + "%" + "</color>";
        }
        else if (newAtckSpdf < player.AttackSpeed)
        {
            newAtckSpd = "<color=#BD0000>" + newAtckSpdf + "</color>";
        }
        else
        {
            newAtckSpd = newAtckSpdf * 100 + "%";
        }

        float newArmorf = (player.BaseArmor + levelDescription.statChanges.armorAdd + player.armorAdd) * player.armorMult * levelDescription.statChanges.armorMult;
        string newArmor;
        if (newArmorf > player.Armor)
        {
            newArmor = "<color=#27FF00>" + newArmorf + "</color>";
        }
        else if (newArmorf < player.Armor)
        {
            newArmor = "<color=#BD0000>" + newArmorf + "</color>";
        }
        else
        {
            newArmor = newArmorf.ToString();
        }

        float newRegenf = (player.BaseRegen + levelDescription.statChanges.regenAdd + player.regenAdd) * player.regenMult * levelDescription.statChanges.regenMult;
        string newRegen;
        if (newRegenf > player.Regen)
        {
            newRegen = "<color=#27FF00>" + newRegenf + "</color>";
        }
        else if (newRegenf < player.Regen)
        {
            newRegen = "<color=#BD0000>" + newRegenf + "</color>";
        }
        else
        {
            newRegen = newRegenf.ToString();
        }

        float newCritChancef = (player.BaseCritChance + levelDescription.statChanges.critChanceAdd + player.critChanceAdd) * player.critChanceMult * levelDescription.statChanges.critChanceMult;
        string newCritChance;
        if (newCritChancef > player.CritChance)
        {
            newCritChance = "<color=#27FF00>" + newCritChancef * 100 + "%" + "</color>";
        }
        else if (newCritChancef < player.CritChance)
        {
            newCritChance = "<color=#BD0000>" + newCritChancef * 100 + "%" + "</color>";
        }
        else
        {
            newCritChance = newCritChancef * 100 + "%";
        }

        float newCritDmgf = (player.BaseCritDamage + levelDescription.statChanges.critDamageAdd + player.critDamageAdd) * player.critDamageMult * levelDescription.statChanges.critDamageMult;
        string newCritDmg;
        if (newCritDmgf > player.CritDamage)
        {
            newCritDmg = "<color=#27FF00>" + newCritDmgf * 100 + "%" + "</color>";
        }
        else if (newCritDmgf < player.CritDamage)
        {
            newCritDmg = "<color=#BD0000>" + newCritDmgf * 100 + "%" + "</color>";
        }
        else
        {
            newCritDmg = newCritDmgf * 100 + "%";
        }

        baseStatsTxt.text =
            "HP: " + player.MaxHp + " -> " + newHP +
            "\nSpeed: " + player.Speed + " -> " + newSpeed +
            "\nDamage: " + player.Damage + " -> " + newDamage +
            "\nAttack Speed: " + player.AttackSpeed * 100 + "%" + " -> " + newAtckSpd +
            "\nArmor: " + player.Armor + " -> " + newArmor +
            "\nRegen: " + player.Regen + " -> " + newRegen +
            "\nCrit Chance: " + (player.CritChance * 100) + "%" + " -> " + newCritChance +
            "\nCrit Damage: " + player.CritDamage * 100 + "%" + " -> " + newCritDmg;

        /*
        if (u.weaponstatChangess.Count > 0)
        {
            Weapon w = player.GetWeapon(u.weaponStatIndex);
            if (w == null)
            {
                w = ResourceManager.GetWeapon(u.weaponStatIndex);
            }
            if (weaponStatsLabel)
            {
                weaponStatsLabel.text = ResourceManager.GetUpgrade(w.upgradeIndex).upgradeName;
            }
            string text = "";
            foreach (string key in w.GetAllStats())
            {
                Upgrade.WeaponstatChanges weaponstatChanges = u.GetWeaponstatChanges(key);

                float newStat = (w.GetStat(key) + weaponstatChanges.add) * weaponstatChanges.mult;
                string newStatTxt;
                if (weaponstatChanges.isPercent)
                {
                    if (newStat > w.GetStat(key))
                    {
                        newStatTxt = "<color=#27FF00>" + newStat * 100 + "%" + "</color>";
                    }
                    else if (newStat < w.GetStat(key))
                    {
                        newStatTxt = "<color=#BD0000>" + newStat * 100 + "%" + "</color>";
                    }
                    else
                    {
                        newStatTxt = newStat * 100 + "%";
                    }
                    text += key + ": " + w.GetStat(key) * 100 + "%" + " -> " + newStatTxt + "\n";
                }
                else
                {
                    if (newStat > w.GetStat(key))
                    {
                        newStatTxt = "<color=#27FF00>" + newStat + "</color>";
                    }
                    else if (newStat < w.GetStat(key))
                    {
                        newStatTxt = "<color=#BD0000>" + newStat + "</color>";
                    }
                    else
                    {
                        newStatTxt = newStat + "";
                    }
                    text += key + ": " + w.GetStat(key) + " -> " + newStatTxt + "\n";
                }
            }
            weaponStatsTxt.text = text;
        }
        else
        {
            weaponStatsTxt.text = "";
            if (weaponStatsLabel)
            {
                weaponStatsLabel.text = "";
            }
        }
        */
    }

    public void OnHoverStop()
    {
        isHover = false;
    }
}
