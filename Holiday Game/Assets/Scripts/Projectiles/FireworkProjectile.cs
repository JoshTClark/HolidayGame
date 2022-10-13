using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkProjectile : BombProjectileBase
{
    public StatsComponent target;
    public float rotationSpeed = 200f;
    private float timer = 0f;
    private bool homing = false;

    // Seeking
    public override void Move()
    {
        Rigidbody2D body = this.gameObject.GetComponent<Rigidbody2D>();
        if (!target.IsDead && homing)
        {
            Vector2 desiredDirection = (Vector2)target.gameObject.transform.position - (Vector2)this.gameObject.transform.position;
            desiredDirection.Normalize();
            float rotateAmount = Vector3.Cross(desiredDirection, transform.right).z;
            body.angularVelocity = -rotateAmount * rotationSpeed;
        }
        body.velocity = transform.right * Speed;
    }

    public override void OnCollision()
    {
    }

    public override void OnHit(StatsComponent receiver)
    {
    }

    // Gets faster over time
    public override void OnUpdate()
    {
        float delta = Time.deltaTime;
        timer += delta;
        if (timer >= 0.3f)
        {
            homing = true;
        }
        SpeedMultiplier += 0.5f * delta;
    }
}