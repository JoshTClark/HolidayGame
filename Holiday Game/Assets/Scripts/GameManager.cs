using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private Enemy enemy;

    [SerializeField]
    private List<Attack> attacks = new List<Attack>();

    public static List<Enemy> currentEnemies = new List<Enemy>();

    public Player player;

    void Start()
    {
        // Just testing adding enemies and an attack to the player
        currentEnemies.Add(Instantiate<Enemy>(enemy, new Vector2(3, 0), Quaternion.identity));
        foreach(Enemy e in currentEnemies)
        {
            e.player = player;
        }
        player.AddAttack(attacks[0]);
    }

    void Update()
    {
        // Moving the camera
        Vector3 camPos = new Vector3(player.transform.position.x, player.transform.position.y, cam.transform.position.z);
        cam.transform.position = camPos;
    }
}
