using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

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

    public int rerolls = 3;

    private float dashSpeed = 65.0f;
    private bool isDash = false;
    private float dashLength = 0.1f;
    private float dashTimer = 0.0f;
    public float dashCooldownTimer = 0.0f;
    private float baseDashCooldown = 5f;
    private float baseDashes = 1f;
    public float currentDashes = 1f;
    private Vector2 dashDirection;
    private int extraDashes = 0;
    private float dashCooldownMultiplier = 1f;
    private float particleTimer = 0.0f;
    private float particleDelay = 0.02f;

    public float DashCooldown { get { return baseDashCooldown * dashCooldownMultiplier; } }
    public float Dashes { get { return baseDashes + extraDashes; } }

    public bool IsInvincible { get { return isInvincible; } }
    public float PickupRange { get { return pickupRange * pickupRangeIncrease; } }

    private List<DashParticle> dashParticles = new List<DashParticle>();
    private ObjectPool<DashParticle> dashParticlePool = new ObjectPool<DashParticle>(createFunc: () => Instantiate<DashParticle>(ResourceManager.playerDashEffect), actionOnGet: (obj) => obj.gameObject.SetActive(true), actionOnRelease: (obj) => obj.gameObject.SetActive(false), actionOnDestroy: (obj) => Destroy(obj.gameObject), collectionCheck: false, defaultCapacity: 10);

    public override void OnStart()
    {
        isInvincible = false;
        iFrames = 0.4f;
        an = gameObject.GetComponent<Animator>();
    }

    public override void OnDeath(DamageInfo info)
    {
    }

    public override void OnUpdate()
    {
        float delta = Time.deltaTime;
        extraDashes = 0;
        dashCooldownMultiplier = 1f;

        if (HasUpgrade(ResourceManager.UpgradeIndex.ExtraDash))
        {
            extraDashes = GetUpgrade(ResourceManager.UpgradeIndex.ExtraDash).CurrentLevel;
        }

        if (currentDashes < Dashes)
        {
            dashCooldownTimer += delta;
            if (dashCooldownTimer >= DashCooldown)
            {
                currentDashes++;
                dashCooldownTimer = DashCooldown;
            }
        }
        else
        {
            dashCooldownTimer = DashCooldown;
        }

        if (isDash)
        {
            // Dash
            isMoving = true;
            isInvincible = true;
            dashTimer += delta;
            particleTimer += delta;
            GetComponent<Rigidbody2D>().velocity = dashDirection * dashSpeed;

            if (particleTimer >= particleDelay)
            {
                particleTimer = 0.0f;
                DashParticle p = dashParticlePool.Get();
                p.SetSprite(GetComponent<SpriteRenderer>().sprite, GetComponent<SpriteRenderer>().flipX);
                p.transform.position = transform.position;
                dashParticles.Add(p);
            }

            if (dashTimer >= dashLength)
            {
                isDash = false;
                dashTimer = 0.0f;
                particleTimer = 0.0f;
                isInvincible = false;
            }
        }
        else
        {
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

        for (int i = dashParticles.Count - 1; i >= 0; i--)
        {
            DashParticle p = dashParticles[i];
            if (p.finished)
            {
                dashParticlePool.Release(p);
                dashParticles.RemoveAt(i);
            }
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
        if (!hitBy.Contains(info.attacker) && !isInvincible)
        {
            // take damage & become invincible
            hitBy.Add(info.attacker);
            invincibilityTimes.Add(iFrames);
            //hitEffect.Play();
            base.TakeDamage(info);
        }
    }
    private void OnDrawGizmos()
    {
        // Showing attack range
        Gizmos.DrawWireSphere(transform.position, attackActivationRange);
        Gizmos.DrawWireSphere(transform.position, PickupRange);
    }

    public void DoDash()
    {
        dashDirection = movement.action.ReadValue<Vector2>();
        if ((dashDirection.x != 0 || dashDirection.y != 0) && !isDash && currentDashes > 0)
        {
            dashCooldownTimer = 0.0f;
            currentDashes--;
            isDash = true;
        }
    }
    virtual protected void OnTriggerStay2D(Collider2D collision)
    {
        HandleCollision(collision);
    }
    private void HandleCollision(Collider2D collision)
    {
        // if the player has SharpShadow Upgrade, and they dash through an enemy
        // Enemy takes [DAMAGE] (should it be scalable? should it be constant? is it just the player's damage stat?)
        if (isDash && HasUpgrade(ResourceManager.UpgradeIndex.SharpShadow))
        {
            // deal the damage
            if (collision.gameObject.GetComponent<Enemy>())
            {
                // Deal Damage to the enemy
                DamageInfo info = new DamageInfo();
                info.damage = Damage;
                info.attacker = this;
                Enemy e = collision.gameObject.GetComponent<Enemy>();
                e.TakeDamage(info);
            }
        }
    }
}
