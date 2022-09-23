using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum WeaponIndex
    {
        None,
        Snowball,
        Test
    }

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private Canvas ui;

    [SerializeField]
    private TMP_Text timerDisplay, difficultyDisplay;

    [SerializeField]
    private Player playerPrefab;

    [SerializeField]
    private List<Weapon> weaponPrefabs = new List<Weapon>();

    private float time = 0.0f;
    private float currentDifficulty = 1;

    public Player Player
    {
        get { return player; }
    }

    private Player player;

    public float CurrentDifficulty
    {
        get { return currentDifficulty; }
    }

    public float GameTime
    {
        get { return time; }
    }

    // Only a single gamemanger should ever exist so we can always get it here
    public static GameManager instance;

    void Start()
    {
        instance = this;

        player = Instantiate<Player>(playerPrefab, new Vector2(), Quaternion.identity);

        // Testing giving player a weapon
        GivePlayerWeapon(WeaponIndex.Snowball);
    }

    void Update()
    {
        // Updating the timer and difficulty
        time += Time.deltaTime;
        currentDifficulty = Mathf.Floor(1 + (time / 5));

        // Updating displays
        string minutes = Mathf.Floor(time / 60).ToString("00");
        string seconds = (time % 60).ToString("00");
        timerDisplay.text = minutes + ":" + seconds;
        difficultyDisplay.text = currentDifficulty.ToString();

        // Moving the camera
        Vector3 camPos = new Vector3(Player.transform.position.x, Player.transform.position.y, cam.transform.position.z);
        cam.transform.position = camPos;
    }

    // Gets a weapon prefab from the list using the index
    public Weapon GetWeaponFromIndex(WeaponIndex index)
    {
        foreach (Weapon i in weaponPrefabs)
        {
            if (i.index == index)
            {
                return i;
            }
        }
        return null;
    }

    // Gives the player the desired weapon
    public void GivePlayerWeapon(WeaponIndex index)
    {
        Weapon weapon = GetWeaponFromIndex(index);
        Player.AddAttack(weapon);
    }

    // Random check
    public static bool RollCheck(float chance)
    {
        float roll = Random.value;
        if (roll < chance)
        {
            return true;
        }
        return false;
    }
}
