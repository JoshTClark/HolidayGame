using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploder : Enemy
{
    private bool inRange = false;
    private float range = 2f; // Test later
    private float deathTimer = 2.5f;
    private float timer = 0.0f;

    public override void OnStart()
    {
        
    }
    public override void OnUpdate()
    {
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
            movements.Add(SeekPlayer() * 1.5f);

        }
    }

    public override void OnDeath(DamageInfo dmgInfo)
    {
        base.OnDeath(dmgInfo);

        // Create an empty explosion
        // Creates an explosion, Check PumpkinBomb.CS for example in Update()
        ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.Explosion);
        //EmptyProjectile explosion = new EmptyProjectile();
        //explosion.index = ResourceManager.ProjectileIndex.Explosion;
        //explosion.projectileTeam = ProjectileBase.Team.Enemy;

    }
}
