using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceManager;

public class Buff : MonoBehaviour
{
    public ResourceManager.BuffIndex index;
    public float totalDamage;
    public DoTick action;
    public bool active = true;
    public StatsComponent afflicting;
    public DamageInfo infoTemplate;
    private float durationTimer = 0.0f;
    private float tickTimer = 0.0f;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (!active) 
        {
            Destroy(this);
        }

        if (GameManager.instance.State == GameManager.GameState.Normal && active)
        {
            durationTimer += Time.deltaTime;

            if (ResourceManager.GetBuffDef(index).type == BuffDef.BuffType.DOT)
            {
                tickTimer += Time.deltaTime;
                if (tickTimer >= ResourceManager.GetBuffDef(index).tickRate)
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

            if (durationTimer >= ResourceManager.GetBuffDef(index).duration)
            {
                active = false;
            }
        }
    }

    public float DamagePerTick()
    {
        return (ResourceManager.GetBuffDef(index).tickRate / ResourceManager.GetBuffDef(index).duration) * totalDamage;
    }

    public delegate void DoTick();
}
