using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceManager;

[CreateAssetMenu(fileName = "Weapon Definition", menuName = Constants.ASSET_MENU_PATH + "Weapon Definition")]
public class WeaponDef : ItemDef
{
    public Weapon weaponPrefab;
}
