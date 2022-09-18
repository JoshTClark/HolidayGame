using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy2 : Enemy
{
    private void Start()
    {
        base.Start();

        AddAttack(GameManager.instance.GetWeaponFromIndex(GameManager.WeaponIndex.Test));
    }

    void Update()
    {
        // Calls the base statscompenent Update
        base.Update();

        // Check for if dead, if so calls Death method
        if (IsDead)
        {
            OnDeath();
        }
        
    }
    public void OnDeath()
    {
        //Drops XP
        Instantiate<XP>(XPType, transform.position, Quaternion.identity);

        //for now just moves the object to another location
        transform.position = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10), 0);

        //Destroy(gameObject); Eventually should remove the object from game
    }
}
