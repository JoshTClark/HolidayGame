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

    private List<StatsComponent> hitBy = new List<StatsComponent>();
    private List<float> invincibilityTimes = new List<float>();
    private float globalInvicibilityTime = 0.1f;
    private float globalInvicibilityTimer = 0f;
    private bool hasGlobalInvicibility = false;

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
    public int maxWeapons = 4;
    public int waitingForLevels = 0;

    public bool godMode = false;

    public GameObject playerDeathEffect;

    public float DashCooldown { get { return baseDashCooldown * dashCooldownMultiplier; } }
    public float Dashes { get { return baseDashes + extraDashes; } }

    public bool IsInvincible { get { return isInvincible; } }
    public float PickupRange { get { return pickupRange * pickupRangeIncrease; } }

    public override void OnStart()
    {
        isInvincible = false;
        iFrames = 0.4f;
        an = gameObject.GetComponent<Animator>();
    }

    public override void OnDeath(DamageInfo info)
    {
        if (playerDeathEffect)
        {
            GameObject gameObject = GameObject.Instantiate<GameObject>(playerDeathEffect);
            gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0f);
        }
        this.gameObject.SetActive(false);
    }

    public override void OnUpdate()
    {
        float delta = Time.deltaTime;
        extraDashes = 0;
        dashCooldownMultiplier = 1f;


        /*
        if (HasUpgrade(ResourceManager.UpgradeIndex.ExtraDash))
        {
            extraDashes = GetItem(ResourceManager.UpgradeIndex.ExtraDash).CurrentLevel;
        }
        */

        //Upgrade Effects

        //Extra Dash Upgrade
        if (HasItem(ResourceManager.ItemIndex.AgilityBoots))
        {
            Item i = GetItem(ResourceManager.ItemIndex.AgilityBoots);
            if (i.HasTakenPath("Swift Strider"))
            {
                if (i.Level == 4)
                {
                    dashCooldownMultiplier *= 0.85f;
                }
                else if (i.Level == 5)
                {
                    dashCooldownMultiplier *= 0.7f;
                }
                else if (i.Level == 6)
                {
                    extraDashes += 1;
                    dashCooldownMultiplier *= 0.6f;
                }

            }


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
            GetComponent<Rigidbody2D>().velocity = dashDirection * dashSpeed;
            GetComponent<TrailEffect>().particleDelay = 0.01f;
            if (dashTimer >= dashLength)
            {
                isDash = false;
                dashTimer = 0.0f;
                isInvincible = false;
            }
        }
        else
        {
            // Basic movement
            if (canMove)
            {
                GetComponent<TrailEffect>().particleDelay = 0.15f;
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
            else
            {
                an.speed = 0;
                isMoving = false;
                GetComponent<Rigidbody2D>().velocity = new Vector2();
            }
        }

        UpdateiFrames();

        pickupRangeIncrease = 1f;
    }

    /// <summary>
    /// Updates
    /// </summary>
    void UpdateiFrames()
    {
        float delta = Time.deltaTime;
        if (hasGlobalInvicibility)
        {
            globalInvicibilityTimer += delta;
            if (globalInvicibilityTimer >= globalInvicibilityTime)
            {
                globalInvicibilityTimer = 0;
                hasGlobalInvicibility = false;
            }
        }
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
        
        if (!hitBy.Contains(info.attacker) && !isInvincible && !godMode && !hasGlobalInvicibility)
        {
            AudioManager.instance.PlaySound("Hurt");
            // take damage & become invincible
            hasGlobalInvicibility = true;
            globalInvicibilityTimer = 0f;
            hitBy.Add(info.attacker);
            invincibilityTimes.Add(iFrames);
            

            if (this.HasItem(ResourceManager.ItemIndex.AgilityBoots))
            {
                Item i = this.GetItem(ResourceManager.ItemIndex.AgilityBoots);
                if(i.HasTakenPath("Berzerker's Boots"))
                {
                    if (i.Level == 4)
                    {
                        info.damage *= 1.1f;
                    }
                    else if (i.Level >= 5)
                    {
                        info.damage *= 1.2f;
                    }
                    
                }
            }

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
        /*
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
        */
    }
}
