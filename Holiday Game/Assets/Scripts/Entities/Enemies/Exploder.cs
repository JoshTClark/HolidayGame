using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploder : Enemy
{
    private bool inRange = false;
    private bool explode = false;
    [SerializeField]
    private float range = 0.01f; // Test later
    private float deathTimer = 0.75f;
    private float timer = 0.0f;

    public override void OnStart()
    {
        
    }
    public override void OnUpdate()
    {
        Move(); 
        if(PlayerDistance() <= range)
        {
            inRange= true;
        }

        // Start explosion stuff
        if (inRange)
        {
            // Start counting down the timer
            timer += Time.deltaTime;

            // Do animation stuff
            if(timer >= deathTimer)
            {
                explode = true; 
                timer = 0.0f;
                // Make a new Damage info
                DamageInfo info = new DamageInfo();
                info.damage = this.MaxHp;
                info.attacker = this;

                TakeDamage(info);
            }
        }
    }
    protected override void CalcMoves()
    {
        if (inRange)
        {
            // Stop movement and explode
            movements.Add(Vector2.zero);
            Velocity = Vector2.zero;
            
        }
        else
        {
            movements.Add(Seek() * 1.5f);

        }
    }

    public override void OnDeath(DamageInfo dmgInfo)
    {
        if(explode && dmgInfo.attacker == this)
        {
            // Create an empty explosion
            // Creates an explosion, Check PumpkinBomb.CS for example in Update()
            BombProjectileBase p = (BombProjectileBase)ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.EnemyBomb);
            p.transform.position = this.transform.position;
            p.Direction = Vector2.zero;
            DamageInfo info = new DamageInfo();
            info.damage = Damage;
            info.attacker = this;
            p.SetDamageInfo(info);
        }

        inRange = false;

        base.OnDeath(dmgInfo);
    }
}
