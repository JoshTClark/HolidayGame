using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballBehavior : ProjectileBase
{
    public FollowingEffect effects;


    private void Start()
    {
        FollowingEffect e = GameObject.Instantiate(effects);
        e.following = this.gameObject;
    }

    public override void Move()
    {
        GetComponent<Rigidbody2D>().velocity = Direction * Speed;
        //Debug.Log(GetComponent<Rigidbody2D>().velocity);
    }

    public override void OnCollision(Collider2D other)
    {
        // Nothing special
    }

    public override void OnDeath()
    {
        // Nothing special
    }

    public override void OnHit(StatsComponent receiver)
    {
        // Nothing special
    }

    public override void OnUpdate()
    {
        if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.Snowballing))
        {
            DamageMultiplier += 0.5f * Time.deltaTime * SpeedMultiplier;
            SizeMultiplier += 0.5f * Time.deltaTime * SpeedMultiplier;
        }
    }

    public override void OnClean()
    {
        FollowingEffect e = GameObject.Instantiate(effects);
        e.following = this.gameObject;
    }
}
