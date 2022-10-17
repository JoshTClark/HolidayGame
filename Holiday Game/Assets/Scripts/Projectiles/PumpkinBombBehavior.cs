using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PumpkinBombBehavior : BombProjectileBase
{
    SpriteRenderer sr;
    private bool isCluster = false;
    public PumpkinBombBehavior selfPrefab;
    private float clusterSpeed = 10f;
    private float slowdownSpeed = 1.25f;
    private float clusterLifetime = 0.8f;
    private float clusterDamage = 0.25f;
    private float clusterSize = 0.8f;

    public void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    public override void Move()
    {
        GetComponent<Rigidbody2D>().velocity = Direction * Speed;
    }

    public override void OnCollision()
    {
        // Nothing special
    }

    public override void OnDeath()
    {
        if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.ClusterPumkins) && !isCluster)
        {
            for (int i = 0; i < 4; i++)
            {
                PumpkinBombBehavior p = Instantiate(selfPrefab, this.transform.position, this.transform.rotation);
                p.isCluster = true;
                p.Speed = clusterSpeed;
                p.Direction = p.transform.right;
                p.RotateDirection(45f + (90f * i));
                p.isCluster = true;
                p.damageInfo = damageInfo.CreateCopy();
                p.DamageMultiplier = clusterDamage * DamageMultiplier;
                p.Lifetime = Lifetime * clusterLifetime;
                float torque = Random.Range(-500f, 500f);
                p.gameObject.GetComponent<Rigidbody2D>().AddTorque(torque);
                p.gameObject.GetComponent<Rigidbody2D>().angularDrag = 1.75f;
                p.SizeMultiplier = clusterSize;
                p.explosionSizeMultiplier = clusterSize * explosionSizeMultiplier;
            }
        }
        base.OnDeath();
    }

    public override void OnHit(StatsComponent receiver)
    {
        // Nothing special
    }

    public override void OnUpdate()
    {
        float delta = Time.deltaTime;
        sr.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(TimeAlive * TimeAlive * 1.2f, 1));

        if (SpeedMultiplier > 0)
        {
            SpeedMultiplier -= delta * slowdownSpeed;
            if (SpeedMultiplier <= 0)
            {
                SpeedMultiplier = 0;
            }
        }

        Rigidbody2D body = this.gameObject.GetComponent<Rigidbody2D>();
        if (body.angularVelocity <= 1f && body.angularVelocity >= -1f)
        {
            body.angularVelocity = 0;
        }
    }
}
