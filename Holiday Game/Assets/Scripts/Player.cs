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

    private float iFrames;
    private bool invincible;
    public bool Invincible { get { return invincible; } }

    public override void OnStart()
    {
        healthBar.SetMaxHealth(MaxHp);
        invincible = false;
        iFrames = 3f;
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
        UpdateiFrames();
    }
    /// <summary>
    /// Updates
    /// </summary>
    void UpdateiFrames()
    {
        // see if we have less health than max
        if(currentHP < MaxHp)
        {
            // See if we should be invincible
            if (invincible)
            {
                // if we're invincible count down the timer
                iFrames -= Time.deltaTime;
            }

            if(iFrames <= 0)
            {
                // Invincible time is up we can take damage again
                // No longer invincible
                invincible = false;

                // Reset iframes
                iFrames = 3f;
            }
            
        }
        else
        {
            invincible = false;
        }
    }
    /// <summary>
    /// Checks if the player can take damage first
    /// </summary>
    /// <param name="damage"></param>
    public override void DealDamage(float damage)
    {
        invincible = true;
        base.DealDamage(damage);
    }
    private void OnDrawGizmos()
    {
        // Showing attack range
        Gizmos.DrawWireSphere(transform.position, attackActivationRange);
    }
}
