using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceManager;

public class BuffInfo
{
    public BuffDef def;
    public float totalDamage;
    public DoTickAction action;
    public bool active = true;
    public StatsComponent afflicting;
    public DamageInfo infoTemplate;
    private float durationTimer = 0.0f;
    private float tickTimer = 0.0f;

    // Update is called once per frame
    public void DoTick()
    {
        if (GameManager.instance.State == GameManager.GameState.MainGame && active)
        {
            durationTimer += Time.deltaTime;

            if (def.type == BuffDef.BuffType.DOT)
            {
                tickTimer += Time.deltaTime;
                if (tickTimer >= def.tickRate)
                {
                    if (action != null)
                    {
                        action();
                    }
                    DamageInfo damageInfo = infoTemplate.CreateCopy();
                    damageInfo.damage = DamagePerTick();
                    afflicting.TakeDamage(damageInfo);
                    tickTimer = 0.0f;
                }
            }

            if (durationTimer >= ResourceManager.GetBuffDef(def.index).duration)
            {
                active = false;
            }
        }
    }

    public float DamagePerTick()
    {
        return (def.tickRate / def.duration * totalDamage);
    }

    public delegate void DoTickAction();
}
