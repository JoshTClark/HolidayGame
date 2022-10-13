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

    public override void OnDeath()
    {
        GameObject gameObject = CreateEmptyProjectile();
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.layer = this.gameObject.layer;
        ProjectileBase projectile = gameObject.GetComponent<ProjectileBase>();
        gameObject.transform.position = this.gameObject.transform.position;
        projectile.SetDamageInfo(damageInfo);
        projectile.DamageMultiplier = DamageMultiplier;
        projectile.Lifetime = explosionLifetime;
        projectile.Pierce = 999f;
        projectile.Size = explosionRadius;
        projectile.SizeMultiplier = 1f;
        projectile.projectileTeam = explosionTeam;
        ParticleSystem effect = GameObject.Instantiate(explosionEffect, this.gameObject.transform.position, Quaternion.identity);
        effect.gameObject.transform.localScale = new Vector3(projectile.Size, projectile.Size);
        effect.Play();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.gameObject.transform.position, explosionRadius);
    }
}
