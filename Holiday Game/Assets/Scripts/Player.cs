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
    private List<StatsComponent> hitBy = new List<StatsComponent>();
    private List<float> invincibilityTimes = new List<float>();


    public bool IsInvincible { get { return isInvincible; } }
    public float PickupRange { get { return pickupRange * pickupRangeIncrease; } }

    public override void OnStart()
    {
        isInvincible = false;
        iFrames = 0.4f;
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

        if (movementInput.x == 0 && movementInput.y == 0)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }

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
        float delta = Time.deltaTime;
        for (int i = hitBy.Count - 1; i >= 0; i--)
        {

            invincibilityTimes[i] -= delta;
            if (invincibilityTimes[i] <= 0)
            {
                hitBy.RemoveAt(i);
                invincibilityTimes.RemoveAt(i);
            }
        }
    }
    /// <summary>
    /// Checks if the player can take damage first
    /// </summary>
    /// <param name="damage"></param>
    public override void TakeDamage(DamageInfo info)
    {
        if (!hitBy.Contains(info.attacker))
        {
            // take damage & become invincible
            hitBy.Add(info.attacker);
            invincibilityTimes.Add(iFrames);
            //hitEffect.Play();
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
