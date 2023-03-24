using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponIcon : MonoBehaviour
{
    public Sprite sprite;
    public Image mask;
    public Image displaySprite;
    public Image timerImage;
    public ResourceManager.WeaponIndex weaponIndex = ResourceManager.WeaponIndex.Null;

    private void Start()
    {
    }

    private void Update()
    {
        displaySprite.sprite = sprite;
        mask.sprite = sprite;
        float percentLeft = 1f;
        foreach (Weapon weapon in GameManager.instance.Player.weapons) 
        {
            if (weapon.hasStarted && weapon.index == this.weaponIndex)
            {
                percentLeft = weapon.PercentTimeLeft();
            }
        }
        timerImage.rectTransform.anchorMax = new Vector2(timerImage.rectTransform.anchorMax.x, 1 - percentLeft);
    }
}
