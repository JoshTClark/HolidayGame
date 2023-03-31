using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PumpkinBombBehavior : BombProjectileBase
{
    SpriteRenderer sr;
    public int clusterNum = 4;
    public bool isCluster = false;
    public bool isRecursive = false;
    public bool shockWave = false;
    public PumpkinBombBehavior selfPrefab;

    public float clusterSpeed = 7.5f;
    private float slowdownSpeed = 1.25f;
    private float clusterLifetime = 0.5f;
    private float clusterDamage = 0.30f;
    private float clusterSize = 0.75f;

    public float shockWaveStunDuration = 0.4f;
    [HideInInspector]
    public float shockWaveSizeMult = 3f;

    public void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    public override void Move()
    {
        GetComponent<Rigidbody2D>().velocity = Direction * Speed;
    }

    public override void OnCollision(Collider2D other)
    {
        // Nothing special
    }

    public override void OnDeath()
    {
        if (isCluster || isRecursive)
        {
            for (int i = 0; i < clusterNum; i++)
            {
                PumpkinBombBehavior p = (PumpkinBombBehavior)pool.Get();
                p.transform.position = this.transform.position;
                p.pool = pool;
                p.isCluster = true;
                p.Speed = clusterSpeed;
                p.Direction = p.transform.right;
                p.RotateDirection((360f/clusterNum/2f) + (360f / clusterNum * i));
                p.isCluster = false;
                if (isRecursive) 
                {
                    p.isCluster = true;
                    p.isRecursive = false;
                    p.clusterNum = 4;
                }
                p.damageInfo = damageInfo.CreateCopy();
                p.DamageMultiplier = clusterDamage * DamageMultiplier;
                p.LifetimeMultiplier = clusterLifetime * LifetimeMultiplier;
                float torque = Random.Range(-500f, 500f);
                p.gameObject.GetComponent<Rigidbody2D>().AddTorque(torque);
                p.gameObject.GetComponent<Rigidbody2D>().angularDrag = 1.75f;
                p.SizeMultiplier = clusterSize * SizeMultiplier;
                p.explosionSizeMultiplier = clusterSize * explosionSizeMultiplier;
            }
        }

        if (shockWave)
        {
            ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.Shockwave);
            p.transform.position = this.transform.position;
            p.Direction = p.transform.right;
            DamageInfo info = this.damageInfo.CreateCopy();
            p.SetDamageInfo(info);
            p.DamageMultiplier = 0;
            p.Pierce = 999;
            ((ShockwaveBehavior)p).stunTime = shockWaveStunDuration;
            ((ShockwaveBehavior)p).maxSizeMult = shockWaveSizeMult;
        }
        base.OnDeath();
    }

    public override void OnHit(StatsComponent receiver, DamageInfo info)
    {
        // Nothing special
    }

    public override void OnUpdate()
    {
        float delta = Time.deltaTime;
        sr.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong((TimeAlive/Lifetime) * (TimeAlive / Lifetime) * 10, 1));

        if (SpeedMultiplier > 0)
        {
            SpeedMultiplier -= delta * slowdownSpeed;
            if (SpeedMultiplier <= 0.01f)
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

    public override void OnClean()
    {
        Rigidbody2D body = this.gameObject.GetComponent<Rigidbody2D>();
        this.transform.rotation = Quaternion.identity;
        explosionSizeMultiplier = 1f;
        body.angularVelocity = 0;
        body.velocity = Vector2.zero;
        this.isCluster = false;
        this.isRecursive = false;
        clusterSpeed = 7.5f;
        clusterNum = 4;
        shockWave = false;
        shockWaveSizeMult = 3f;
        shockWaveStunDuration = 1.0f;
        this.gameObject.SetActive(true);
    }
}
