using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    private float amount = 1f;

    void Update()
    {
        amount -= Time.deltaTime;
        gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Amount", amount);
        if (amount < 0) 
        {
            Destroy(gameObject);
        }
    }
}
