using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceManager;

[CreateAssetMenu(fileName = "BuffDef Config", menuName = Constants.ASSET_MENU_PATH + "BuffDef Config")]
public class BuffDef : ScriptableObject
{
    public enum BuffType
    {
        Debuff,
        Buff,
        DOT
    }
    public ResourceManager.BuffIndex index;
    public BuffType type;
    public string buffName;
    public float duration;
    public float tickRate;
    public FollowingEffectIndex effect;
}
