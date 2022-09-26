using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Player now derives from StatsComponent. This helps it keep track of all of its stats and helps organize things a lot
public class Player : StatsComponent
{
    [SerializeField]
    private InputActionReference movement;

    public HealthBar healthBar;

    public float pickupRange;
    public float attackActivationRange;

    public override void OnStart()
    {
        healthBar.SetMaxHealth(MaxHp);
    }

    public override void OnDeath()
    {
    }

    public override void OnUpdate()
    {
        float delta = Time.deltaTime;

        // Basic movement
        Vector2 movementInput = movement.action.ReadValue<Vector2>();
        movementInput = movementInput * Speed;
        GetComponent<Rigidbody2D>().velocity = movementInput;

        healthBar.SetHealth(CurrentHP);
    }

    private void OnDrawGizmos()
    {
        // Showing attack range
        Gizmos.DrawWireSphere(transform.position, attackActivationRange);
    }
}
