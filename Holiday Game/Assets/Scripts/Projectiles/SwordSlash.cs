using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : ProjectileBase
{
    public bool hasTipper = false;
    public GameObject tip;
    private float degrees = 180f;
    private float moved = 0f;

    public override void Move()
    {
        if (degrees > moved)
        {
            float rotate = degrees / (Lifetime / 3) * Time.deltaTime;
            this.transform.Rotate(new Vector3(0, 0, rotate));
            moved += rotate;
        }
    }

    public override void OnClean()
    {
        this.gameObject.SetActive(true);
        moved = 0;
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
        Debug.Log(Vector2.Distance(closest, this.gameObject.transform.position));
        Item i = info.attacker.GetItem(ResourceManager.ItemIndex.SwordWeapon);
        if (i.Level >= 4 && Vector2.Distance(closest, this.gameObject.transform.position) >= (2.5f * SizeMultiplier)) 
        {
            info.critOveride = true;
            info.damage *= 1.5f;
        }
    }

    public override void OnUpdate()
    {
        /*
        speedMult = 50 * ((Lifetime - (TimeAlive*2)) / Lifetime);
        speedMult = Mathf.Clamp(speedMult, 0, 100);
        */
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0, 0, 0.25f);
        Gizmos.DrawSphere(this.gameObject.transform.position, (2.5f * SizeMultiplier));
    }
}
