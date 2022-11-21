using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow : Enemy
{
    public override void OnStart()
    {
    }

    public override void OnUpdate()
    {
        if (!HasBuff(ResourceManager.BuffIndex.Stunned))
        {
            CalcMoves();
        }
        // Then take Velocity normalize it so it's a heading vector
        // scale that by speed, then apply movement
        Vector2 velocity = Vector2.zero;
        if (movements.Count > 0)
        {
            foreach (Vector2 v in movements)
            {
                velocity += v;
            }
        }
        Vector2 knockbackAmount = Vector2.zero;
        if (knockback.Count > 0)
        {
            foreach (Vector2 v in knockback)
            {
                knockbackAmount += v;
            }
        }

        GetComponent<Rigidbody2D>().velocity = (velocity.normalized * Speed);
        GetComponent<Rigidbody2D>().position = GetComponent<Rigidbody2D>().position + (knockbackAmount);
        movements.Clear();
        knockback.Clear();
    }

    protected override void CalcMoves()
    {
        Rigidbody2D body = this.gameObject.GetComponent<Rigidbody2D>();
        if (!player.IsDead)
        {
            Vector2 desiredDirection = (Vector2)player.gameObject.transform.position - (Vector2)this.gameObject.transform.position;
            desiredDirection.Normalize();
            float rotateAmount = Vector3.Cross(desiredDirection, -transform.right).z;
            body.angularVelocity = -rotateAmount * 90f;
        }

        movements.Add(-this.transform.right);
    }
}
