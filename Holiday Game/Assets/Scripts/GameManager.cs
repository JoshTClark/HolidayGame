using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// can be called from anywhere !!!!!

public class GameManager : MonoBehaviour
{
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
    private TMP_Text timerDisplay, difficultyDisplay, playerStats, playerLevel;

    [SerializeField]
    private Image xpBar;

    [SerializeField]
    private CanvasRenderer statsPanel, pausedPanel, gamePanel, upgradePanel;

    [SerializeField]
    private float timeToDifficultyIncrease;

    [SerializeField]
    private InputActionReference displayStats, pauseGame;

    [SerializeField]
    private HealthBar healthBar;

    private List<Weapon> weaponPrefabs;
    private List<Upgrade> upgradeDefinitions;

    private float time = 0.0f;
    private float currentDifficulty = 1;
    private GameState state = GameState.Normal;
    private bool paused = false;

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

    public GameState State
    {
        get { return state; }
    }

    // Only a single gamemanger should ever exist so we can always get it here
    public static GameManager instance;

    void Start()
    {
        instance = this;
        ResourceManager.Init();

        pauseGame.action.performed += (InputAction.CallbackContext callback) =>
        {
            paused = !paused;
        };
        pausedPanel.gameObject.SetActive(false);
        upgradePanel.gameObject.SetActive(false);

        player = Instantiate<Player>(ResourceManager.playerPrefab, new Vector2(), Quaternion.identity);
        player.healthBar = healthBar;
        weaponPrefabs = ResourceManager.weaponPrefabs;
        upgradeDefinitions = ResourceManager.upgradeDefinitions;

        // Testing giving player a weapon
        GivePlayerWeapon(ResourceManager.WeaponIndex.Snowball);
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
                string minutes = Mathf.Floor(time / 60).ToString();
                string seconds = (time % 60).ToString("00");
                timerDisplay.text = minutes + ":" + seconds;
                difficultyDisplay.text = currentDifficulty.ToString();
                float xpAmount = Mathf.Clamp((player.GetPercentToNextLevel() * 0.8f) + 0.1f, 0.1f, 0.9f);
                xpBar.GetComponent<RectTransform>().anchorMax = new Vector2(xpAmount, xpBar.GetComponent<RectTransform>().anchorMax.y);
                playerLevel.text = player.Level.ToString();

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

                // Pause screen
                if (paused)
                {
                    state = GameState.Paused;
                    pausedPanel.gameObject.SetActive(true);
                }
                break;
            case GameState.Paused:
                if (!paused)
                {
                    state = GameState.Normal;
                    pausedPanel.gameObject.SetActive(false);
                }
                break;
            case GameState.UpgradeMenu:
                break;
        }
    }

    // Gets a weapon prefab from the list using the index
    public Weapon GetWeaponFromIndex(ResourceManager.WeaponIndex index)
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
    public void GivePlayerWeapon(ResourceManager.WeaponIndex index)
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

    public Upgrade GetUpgrade(ResourceManager.UpgradeIndex index)
    {
        foreach (Upgrade i in upgradeDefinitions)
        {
            if (i.index == index)
            {
                return i;
            }
        }
        return null;
    }
}
