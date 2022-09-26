using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public enum WeaponIndex
    {
        Snowball,
        Test,
        Count
    }

    public enum GameState
    {
        Normal,
        Paused,
        UpgradeMenu
    }

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private Canvas ui;

    [SerializeField]
    private TMP_Text timerDisplay, difficultyDisplay, playerStats, pausedText;

    [SerializeField]
    private CanvasRenderer statsPanel;

    [SerializeField]
    private Player playerPrefab;

    [SerializeField]
    private float timeToDifficultyIncrease;

    [SerializeField]
    private List<Weapon> weaponPrefabs = new List<Weapon>();

    [SerializeField]
    private InputActionReference displayStats;

    private float time = 0.0f;
    private float currentDifficulty = 1;
    private GameState state = GameState.Normal;

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
        switch (state)
        {
            case GameState.Normal:
                // Updating the timer and difficulty
                time += Time.deltaTime;
                currentDifficulty = Mathf.Floor(1 + (time / timeToDifficultyIncrease));

                // Updating displays
                string minutes = Mathf.Floor(time / 60).ToString("00");
                string seconds = (time % 60).ToString("00");
                timerDisplay.text = minutes + ":" + seconds;
                difficultyDisplay.text = currentDifficulty.ToString();
                pausedText.gameObject.SetActive(false);

                // Displaying stats
                if (displayStats.action.ReadValue<float>() > 0)
                {
                    statsPanel.gameObject.SetActive(true);
                    DisplayPlayerStats();
                }
                else
                {
                    statsPanel.gameObject.SetActive(false);
                }

                // Moving the camera
                Vector3 camPos = new Vector3(Player.transform.position.x, Player.transform.position.y, cam.transform.position.z);
                cam.transform.position = camPos;
                break;
            case GameState.Paused:
                pausedText.gameObject.SetActive(true);
                break;
            case GameState.UpgradeMenu:
                break;
        }
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

    private void DisplayPlayerStats()
    {
        playerStats.text =
            "Max HP = " + player.MaxHp +
            "\nSpeed = " + player.Speed +
            "\nDamage = " + player.Damage +
            "\nAttack Speed = " + player.AttackSpeed +
            "\nArmor = " + player.Armor +
            "\nRegen = " + player.Regen +
            "\nCritical Chance = " + player.CritDamage +
            "\nCritical Damage = " + player.CritDamage;
    }
}
