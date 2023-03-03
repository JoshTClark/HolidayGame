using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CrowWeapon : Weapon
{
    public override void CalcStats()
    {
    }

    public override void OnUpdate()
    {
        float delta = Time.deltaTime;

        Player player = GameManager.instance.Player;
        Vector2 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Attack timer it will do the attack every "delay" seconds
        if (canFire)
        {
            int ran = Random.Range(1, 3);
            for (int i = 0; i < ran; i++)
            {
                Enemy e = EnemyManager.instance.SpawnEnemy(ResourceManager.EnemyIndex.Crow, this.transform.position);
                e.gameObject.GetComponent<Rigidbody2D>().SetRotation(Random.Range(0, 360));
            }
            ResetTimer();
        }
    }
    protected override void WeaponSound()
    {
        return;
    }
}
