using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceManager;

public class BuffInfo
{
    public BuffIndex index;
    public bool isDebuff;
    public float duration;
    public float chance = 1f;
}

public class DotInfo : BuffInfo
{
    public float tickRate;
    public float damagePerTick;
}
