using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerCollider : MonoBehaviour
{
    public Enemy parentEnemy;

    virtual protected void OnTriggerStay2D(Collider2D collision)
    {
        parentEnemy.HandleCollision(collision);
    }
}
