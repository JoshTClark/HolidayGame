using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Player now derives from StatsComponent. This helps it keep track of all of its stats and helps organize things a lot
public class Player : StatsComponent
{
    [SerializeField]
    private InputActionReference movement;
    [SerializeField]
    private float pickupRange;
    private float pickupRangeIncrease;
    public float attackActivationRange;
    [SerializeField]
    private float iFrames;
    private bool isInvincible;

    private Animator an;

    [SerializeField]
    private AudioSource hitEffect;

    
    public bool IsInvincible { get { return isInvincible; } }
    public float PickupRange { get { return pickupRange * pickupRangeIncrease; } }

    public override void OnStart()
    {
        isInvincible = false;
        iFrames = .5f;
        an = gameObject.GetComponent<Animator>();
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
        if (movementInput != Vector2.zero)
        {
            an.speed = 1;
            if (movementInput.x > 0)
            {
                sr.flipX = true;
            }
            if (movementInput.x < 0)
            {
                sr.flipX = false;
            }
        }
        else
        {
            an.speed = 0;
        }

        UpdateiFrames();

        pickupRangeIncrease = 1f;
        if (HasUpgrade(ResourceManager.UpgradeIndex.XP1))
        {
            pickupRangeIncrease += 0.05f * GetUpgrade(ResourceManager.UpgradeIndex.XP1).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.XP2))
        {
            pickupRangeIncrease += 0.1f * GetUpgrade(ResourceManager.UpgradeIndex.XP2).CurrentLevel;
        }
        if (HasUpgrade(ResourceManager.UpgradeIndex.XP3))
        {
            pickupRangeIncrease += 0.15f * GetUpgrade(ResourceManager.UpgradeIndex.XP3).CurrentLevel;
        }
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
    public override void TakeDamage(DamageInfo info)
    {
        if (isInvincible)
        {
            UpdateiFrames();
        }
        else
        {
            hitEffect.Play();
            // take damage & become invincible
            base.TakeDamage(info);
            isInvincible = true;
        }
    }
    private void OnDrawGizmos()
    {
        // Showing attack range
        Gizmos.DrawWireSphere(transform.position, attackActivationRange);
        Gizmos.DrawWireSphere(transform.position, PickupRange);
    }
}
