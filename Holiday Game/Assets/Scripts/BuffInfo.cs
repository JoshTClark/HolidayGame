using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceManager;

public class BuffInfo
{
    public BuffIndex index;
    public bool isDebuff;
    public float duration;
}

public class DotInfo : BuffInfo
{
    public float tickRate;
    public float damagePerTick;
}
