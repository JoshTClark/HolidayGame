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
    [SerializeField]
    private float iFrames;
    private bool isInvincible;
    public bool IsInvincible { get { return isInvincible; } }

    public override void OnStart()
    {
        healthBar.SetMaxHealth(MaxHp);
        isInvincible = false;
        iFrames = .5f;
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

        healthBar.SetMaxHealth(MaxHp);
        healthBar.SetHealth(CurrentHP);
        UpdateiFrames();
    }
    /// <summary>
    /// Updates
    /// </summary>
    void UpdateiFrames()
    {
        // See if we should be invincible
        if (isInvincible)
        {
            // if we're invincible count down the timer
            iFrames -= Time.deltaTime;
        }

        if (iFrames <= 0)
        {
            // Invincible time is up we can take damage again
            // No longer invincible
            isInvincible = false;

            // Reset iframes
            iFrames = 3f;
        }
    }
    /// <summary>
    /// Checks if the player can take damage first
    /// </summary>
    /// <param name="damage"></param>
    public override void DealDamage(DamageInfo info)
    {
        if (isInvincible)
        {
            UpdateiFrames();
        }
        else
        {
            // take damage & become invincible
            base.DealDamage(info);
            isInvincible = true;
        }
    }
    private void OnDrawGizmos()
    {
        // Showing attack range
        Gizmos.DrawWireSphere(transform.position, attackActivationRange);
    }
}
