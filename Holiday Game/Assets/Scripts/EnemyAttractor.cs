using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttractor : MonoBehaviour
{
    [HideInInspector]
    public float Radius
    {
        set { gameObject.GetComponent<CircleCollider2D>().radius = value; }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            other.gameObject.GetComponent<Enemy>().attractor = this.gameObject;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.gameObject.transform.position, gameObject.GetComponent<CircleCollider2D>().radius);
    }
}
