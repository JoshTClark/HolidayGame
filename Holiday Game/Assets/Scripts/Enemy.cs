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

    public void DealDamage(float damage)
    {
        health -= damage;
    }
}
