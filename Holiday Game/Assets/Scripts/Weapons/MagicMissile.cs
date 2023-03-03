using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : Weapon
{
    private bool doVolley = false;
    private float volleyTimer = 0.2f;
    private float shotDelay = 0.6f;
    private int firedShots = 0;
    private bool hasFired = false;

    public override void CalcStats()
    {
        /*
        if (owner.HasUpgrade(ResourceManager.UpgradeIndex.CandyCornSpray))
        {
            trueStats["Attack Speed"] = 1.5f + 0.5f * (owner.GetItem(ResourceManager.UpgradeIndex.CandyCornSpray).CurrentLevel - 1);
        }

        float count = GetBaseStat("Projectiles");

        // More fireworks upgrade
        if (GameManager.instance.Player.HasUpgrade(ResourceManager.UpgradeIndex.MoreCorn))
        {
            count += 1f * (GameManager.instance.Player.GetItem(ResourceManager.UpgradeIndex.MoreCorn).CurrentLevel);
        }

        trueStats["Projectiles"] = count;
        */
    }

    public override void OnUpdate()
    {

        Vector3 target = MousePosition();
        if (!doVolley)
        {
            Vector2 direction = target - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (canFire)
        {
            doVolley = true;
        }

        if (doVolley)
        {
            
            if (firedShots < GetStat("Projectiles"))
            {
                volleyTimer += Time.deltaTime;
                if (volleyTimer >= (((shotDelay / GetStat("Projectiles")) / owner.AttackSpeed) / GetStat("Attack Speed")))
                {
                    
                    float accuracyOff = 0;
                    /*
                    if (owner.HasUpgrade(ResourceManager.UpgradeIndex.CandyCornSpray))
                    {
                        accuracyOff = Random.Range(-10 * (owner.GetItem(ResourceManager.UpgradeIndex.CandyCornSpray).CurrentLevel), 10 * (owner.GetItem(ResourceManager.UpgradeIndex.CandyCornSpray).CurrentLevel));
                    }
                    */
                    ProjectileBase p = ProjectileManager.GetProjectile(ResourceManager.ProjectileIndex.MagicMissile);
                    p.transform.position = this.transform.position;
                    p.transform.rotation = transform.rotation;
                    p.Direction = transform.right;
                    p.Pierce = GetStat("Pierce");
                    p.RotateDirection(accuracyOff);
                    DamageInfo info = new DamageInfo();
                    info.damage = owner.Damage * GetStat("Damage");
                    info.attacker = owner;
                    info.knockbackDirection = p.Direction;
                    info.knockback = 0.2f;
                    info.weapon = ResourceManager.WeaponIndex.CandyCornRifle;
                    p.SetDamageInfo(info);
                    //float torque = 500f;
                    //p.gameObject.GetComponent<Rigidbody2D>().AddTorque(torque);
                    firedShots++;
                    volleyTimer = 0.0f;

                    if (!source.isPlaying || !hasFired)
                    {
                        hasFired= true;
                        source.Stop();
                        WeaponSound();
                    }
                }
            }
            else
            {
                ResetTimer();
                doVolley = false;
                volleyTimer = 0.2f;
                firedShots = 0;
                hasFired= false;    
            }
        }
    }

    /// <summary>
    /// Randomly chooses a sound clip from the list and plays it once
    /// </summary>
    protected override void WeaponSound()
    {
        AudioClip clip = audioClips[Random.Range(0, audioClips.Count)];
        source.PlayOneShot(clip);
    }
}
