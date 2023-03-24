using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MagicMissileProjectile : ProjectileBase
{
    public bool seeking = false;

    public override void Move()
    {
        Rigidbody2D body = this.gameObject.GetComponent<Rigidbody2D>();
        if (seeking)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePosition.z = 0.0f;
            Vector2 desiredDirection = (Vector2)mousePosition - (Vector2)this.gameObject.transform.position;
            desiredDirection.Normalize();
            float rotateAmount = Vector3.Cross(desiredDirection, transform.right).z;
            body.angularVelocity = -rotateAmount * 400f;
        }
        body.velocity = transform.right * Speed;
    }

    public override void OnClean()
    {
        Rigidbody2D body = this.gameObject.GetComponent<Rigidbody2D>();
        this.transform.rotation = Quaternion.identity;
        body.angularVelocity = 0;
        body.velocity = Vector2.zero;
        seeking = false;
    }

    public override void OnCollision(Collider2D other)
    {
    }

    public override void OnDeath()
    {
    }

    public override void OnHit(StatsComponent receiver, DamageInfo info)
    {
    }

    public override void OnUpdate()
    {
    }
}
