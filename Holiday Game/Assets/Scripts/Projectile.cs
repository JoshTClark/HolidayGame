using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float timeAlive = 0.0f;

    [SerializeField]
    private float speed, damage, lifetime;

    void Update()
    {
        float delta = Time.deltaTime;

        timeAlive += delta;
        if (timeAlive >= lifetime)
        {
            Destroy(gameObject);
        }

        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }
}
