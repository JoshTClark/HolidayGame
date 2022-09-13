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

    public Player player;

    void Start()
    {
        currentEnemies.Add(Instantiate<Enemy>(enemy, new Vector2(3, 0), Quaternion.identity));
        foreach(Enemy e in currentEnemies)
        {
            e.player = player;
        }
        player.AddAttack(attacks[0]);
    }

    void Update()
    {

    }
}
