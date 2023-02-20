using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Scarecrow : Enemy
{
    public Light2D lights;

    public override void OnUpdate()
    {
        if (GameManager.instance.currentHour >= 6 && GameManager.instance.currentHour <= 18)
        {
            lights.enabled = false;
        }
        else 
        {
            lights.enabled = true;
        }
        Move();
    }
    protected override void CalcMoves()
    {
        movements.Add(SeekPlayer() * 1.5f);
        movements.Add(Separation() * 2f);
    }

    public override void OnStart()
    {
        // Set player ranges
        minPlayerDist = 6f;
        maxPlayerDist = 8f;

        AddWeapon(ResourceManager.GetWeapon(ResourceManager.WeaponIndex.Crows));
    }
}
