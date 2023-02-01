using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrbitalBase : MonoBehaviour
{
    public float speed = 1f;
    public float distance = 5f;
    public OrbitalParent parent;

    private void Update()
    {
        float delta = Time.deltaTime;
        transform.localPosition = new Vector2(distance, 0);

        parent.transform.Rotate(new Vector3(0, 0, delta * speed * Mathf.Rad2Deg));

        OnUpdate();
    }

    protected Enemy GetClosestEnemy()
    {
        List<Enemy> enemies = EnemyManager.instance.AllEnemies;
        if (enemies.Count > 0)
        {
            Enemy closest = null;
            float distance = GameManager.instance.Player.attackActivationRange;
            foreach (Enemy e in enemies)
            {
                float newDistance = e.PlayerDistance();
                if (newDistance < distance)
                {
                    closest = e;
                    distance = newDistance;
                }
            }
            return closest;
        }
        else
        {
            return null;
        }
    }

    public abstract void OnUpdate();
}
