using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BombProjectileBase : ProjectileBase
{
    public bool explodeOnContact;
    public float explosionRadius;
    public float explosionLifetime;
    public Team explosionTeam;
    public ParticleSystem explosionEffect;
    public float explosionSizeMultiplier = 1f;
    public GameObject explosionPrefab;

    public override void OnDeath()
    {
        GameObject gameObject = GetEmptyProjectile();
        if (!gameObject.GetComponent<SpriteRenderer>())
        {
            gameObject.AddComponent<SpriteRenderer>();
        }
        gameObject.layer = this.gameObject.layer;
        ProjectileBase projectile = gameObject.GetComponent<ProjectileBase>();
        gameObject.transform.position = this.gameObject.transform.position;
        DamageInfo info = damageInfo.CreateCopy();
        info.radialKnockback = true;
        info.damagePos = this.transform.position;
        projectile.SetDamageInfo(info);
        projectile.DamageMultiplier = DamageMultiplier;
        projectile.Lifetime = explosionLifetime;
        projectile.Pierce = 999f;
        projectile.Size = explosionRadius;
        projectile.SizeMultiplier = explosionSizeMultiplier;
        projectile.projectileTeam = explosionTeam;
        if (explosionEffect)
        {
            ParticleSystem effect = GameObject.Instantiate(explosionEffect, this.gameObject.transform.position, Quaternion.identity);
            effect.gameObject.transform.localScale = new Vector3(projectile.Size, projectile.Size);
            effect.Play();
        }
        if (explosionPrefab)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosion.GetComponent<SpriteRenderer>().size = new Vector3(explosionRadius, explosionRadius);
            explosion.gameObject.transform.localScale = new Vector3(projectile.Size, projectile.Size);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.gameObject.transform.position, explosionRadius * explosionSizeMultiplier);
    }
}
