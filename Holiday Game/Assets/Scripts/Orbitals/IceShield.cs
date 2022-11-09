using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShield : OrbitalBase
{
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.gameObject.GetComponent<ProjectileBase>())
        {
            ProjectileBase p = other.collider.gameObject.GetComponent< ProjectileBase>();
            if (p.projectileTeam == ProjectileBase.Team.Enemy)
            {
                p.GetComponent<Rigidbody2D>().velocity = Vector3.Reflect(p.GetComponent<Rigidbody2D>().velocity, other.GetContact(0).normal);
                p.Direction = Vector3.Reflect(p.Direction, other.GetContact(0).normal);
                p.projectileTeam = ProjectileBase.Team.Player;
                p.LifetimeMultiplier *= 2f;
            }
        }
    }
}
