using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Enemy enemy;

    [SerializeField]
    private List<Attack> attacks = new List<Attack>();

    public static List<Enemy> currentEnemies = new List<Enemy>();

    public static Player player;

    void Start()
    {
        // Just testing adding enemies and an attack to the player
        currentEnemies.Add(Instantiate<Enemy>(enemy, new Vector2(3, 0), Quaternion.identity));
        player.AddAttack(attacks[0]);
    }

    void Update()
    {

    }
}
