using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinBomb : Weapon
{
    public override void OnUpdate()
    {
        float delta = Time.deltaTime;

        if (CanFire)
        {
            ProjectileBase p = Instantiate<ProjectileBase>(projectile, transform.position, Quaternion.identity);
            p.Direction = Vector2.zero;
            ResetTimer();
        }
    }

}
