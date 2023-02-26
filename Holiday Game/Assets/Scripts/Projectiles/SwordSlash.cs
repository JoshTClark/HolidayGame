using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : ProjectileBase
{
    public bool hasTipper = false;
    private float degrees = 180f;
    private float moved = 0f;
    private bool inverse = false;
    public bool spin = false;
    public SpriteRenderer sprite;

    public override void Move()
    {
        if (spin)
        {
            if (degrees * 2 > moved)
            {
                float rotate = (degrees * 2) / (Lifetime / 2) * Time.deltaTime;
                this.transform.Rotate(new Vector3(0, 0, rotate));
                moved += rotate;
            }
        }
        else if (inverse)
        {
            if (degrees > moved)
            {
                float rotate = degrees / (Lifetime / 2) * Time.deltaTime;
                this.transform.Rotate(new Vector3(0, 0, -rotate));
                moved += rotate;
            }
        }
        else
        {
            if (degrees > moved)
            {
                float rotate = degrees / (Lifetime / 2) * Time.deltaTime;
                this.transform.Rotate(new Vector3(0, 0, rotate));
                moved += rotate;
            }
        }
    }

    public override void OnClean()
    {
        this.gameObject.SetActive(true);
        moved = 0;
        inverse = false;
        sprite.flipX = false;
        spin = false;
    }

    public override void OnCollision(Collider2D other)
    {
    }

    public override void OnDeath()
    {
    }

    public override void OnHit(StatsComponent receiver, DamageInfo info)
    {
        Vector2 closest = info.otherCollider.ClosestPoint(this.gameObject.transform.position);
        //Debug.Log(Vector2.Distance(closest, this.gameObject.transform.position));
        Item i = info.attacker.GetItem(ResourceManager.ItemIndex.SwordWeapon);
        if (i.Level >= 4 && Vector2.Distance(closest, this.gameObject.transform.position) >= (2.5f * SizeMultiplier))
        {
            info.critOveride = true;
            info.damage *= 1.5f;
        }
    }

    public override void OnUpdate()
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0, 0, 0.25f);
        Gizmos.DrawSphere(this.gameObject.transform.position, (2.5f * SizeMultiplier));
    }

    public void InvertSwing()
    {
        inverse = true;
        this.transform.Rotate(new Vector3(0, 0, degrees));
        sprite.flipX = true;
    }
}
