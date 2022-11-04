using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    [SerializeField]
    //protected AudioSource spawnSound;
    public override void OnStart()
    {
        AddAttack(ResourceManager.GetWeapon(ResourceManager.WeaponIndex.BossAttack));
        AddAttack(ResourceManager.GetWeapon(ResourceManager.WeaponIndex.BossAttack2));
    }

    public override void OnUpdate()
    {
        Move();
    }

    protected override void CalcMoves()
    {
        movements.Add(SeekPlayer());
    }
}
