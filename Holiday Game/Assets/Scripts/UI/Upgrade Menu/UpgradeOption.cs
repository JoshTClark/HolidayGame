using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeOption : MonoBehaviour
{
    public TMP_Text nameLabel, levelLabel, descLabel, statsLabel;
    public Image icon, iconOutline;
    public bool isHover = false;
    private float scale = 1f;
    private float maxScale = 1.025f;
    private float scaleSpeed = 3f;

    public void SetItem(ItemDef item, int level, ItemDef.LevelDescription levelDescription)
    {
        // Setting the icon
        if (item.icon)
        {
            icon.sprite = item.icon;
        }

        // Setting the color
        levelLabel.enableVertexGradient = false;
        nameLabel.enableVertexGradient = false;
        Color color = item.TierColor();
        iconOutline.color = color;
        levelLabel.color = color;
        nameLabel.color = color;

        if (item.tier == ItemDef.Tier.Epic || item.tier == ItemDef.Tier.Legendary)
        {
            VertexGradient grad = new VertexGradient();
            grad.topLeft = color * 1.2f;
            grad.bottomLeft = color;
            grad.topRight = color;
            grad.bottomRight = color * 0.8f;
            levelLabel.enableVertexGradient = true;
            nameLabel.enableVertexGradient = true;
            levelLabel.color = new Color(1f, 1f, 1f, 1f);
            nameLabel.color = new Color(1f, 1f, 1f, 1f);
            levelLabel.colorGradient = grad;
            nameLabel.colorGradient = grad;
            iconOutline.color = color;
        }

        isHover = false;
        nameLabel.text = item.itemName;
        descLabel.text = levelDescription.desc;
        statsLabel.text = "";
        if (levelDescription.statChanges.Count > 0)
        {
            statsLabel.text += levelDescription.statChanges[0];
            for (int i = 1; i < levelDescription.statChanges.Count; i++)
            {
                statsLabel.text += "\n" + levelDescription.statChanges[i];
            }
        }

        if (level == 0)
        {
            if (item.GetType() == typeof(WeaponDef))
            {
                levelLabel.text = "New weapon!";
            }
            else
            {
                levelLabel.text = "New item!";
            }
        }
        else
        {
            levelLabel.text = "Level: " + (level + 1);
        }
    }

    public void Update()
    {
        float delta = Time.unscaledDeltaTime;

        // Hover animation
        RectTransform rectTransform = this.gameObject.GetComponent<RectTransform>();
        if (isHover && scale < maxScale)
        {
            scale += scaleSpeed * delta;
            if (scale > maxScale)
            {
                scale = maxScale;
            }
        }
        else if (!isHover && scale > 1f)
        {
            scale -= scaleSpeed * delta;
            if (scale < 1f)
            {
                scale = 1f;
            }
        }

        rectTransform.localScale = new Vector3(scale, scale, 1);
    }

    /// <summary>
    /// Called when player hovers over the button
    /// </summary>
    public void OnHoverStart()
    {
        isHover = true;
        /*
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
