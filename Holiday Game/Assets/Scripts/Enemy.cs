using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float health;

    void Update()
    {

    }

    // Deals damage to the enemy
    public void DealDamage(float damage)
    {
        health -= damage;
    }
}
