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
        UpgradeMenu,
        Title,
        GameOver
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
    private CanvasRenderer statsMenu, playerStatsPanel, pausedPanel, gamePanel, upgradePanel, titlePanel, gameOverPanel;

    [SerializeField]
    private float timeToDifficultyIncrease;

    [SerializeField]
    private InputActionReference displayStats, pauseGame, giveXP;

    [SerializeField]
    private HealthBar healthBar;

    private float time = 0.0f;
    private float currentDifficulty = 1;
    private GameState state = GameState.Title;
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

        giveXP.action.performed += (InputAction.CallbackContext callback) =>
        {
            player.AddXP(player.GetXpToNextLevel() - player.XP + 1);
        };

        pausedPanel.gameObject.SetActive(false);
        upgradePanel.gameObject.SetActive(false);
        gamePanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);
        titlePanel.gameObject.SetActive(true);
    }

    void Update()
    {
        switch (state)
        {
            case GameState.Title:
                titlePanel.gameObject.SetActive(true);
                break;
            case GameState.GameOver:
                gamePanel.gameObject.SetActive(false);
                gameOverPanel.gameObject.SetActive(true);
                break;
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
                    statsMenu.gameObject.SetActive(true);
                    DisplayStats();
                }
                else
                {
                    statsMenu.gameObject.SetActive(false);
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

                // GameOver
                if (player.IsDead)
                {
                    state = GameState.GameOver;
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
                upgradePanel.gameObject.SetActive(true);
                UpgradePanelManager upgradeManager = upgradePanel.GetComponent<UpgradePanelManager>();
                if (!upgradeManager.displaying)
                {
                    upgradeManager.player = player;
                    upgradeManager.SetUpgradesByPools(GetPossiblePools(), 4);
                    upgradeManager.ShowOptions();
                }
                if (upgradeManager.selected)
                {
                    state = GameState.Normal;
                    upgradePanel.gameObject.SetActive(false);
                    upgradeManager.Clear();
                }
                break;
        }
    }

    // Gives the player the desired weapon
    public void GivePlayerWeapon(ResourceManager.WeaponIndex index)
    {
        Weapon weapon = ResourceManager.GetWeapon(index);
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

    private void DisplayStats()
    {
        playerStats.text =
            "Max HP: " + player.MaxHp +
            "\nSpeed: " + player.Speed +
            "\nDamage: " + player.Damage +
            "\nAttack Speed:" + player.AttackSpeed +
            "\nArmor: " + player.Armor +
            "\nRegen: " + player.Regen +
            "\nCrit Chance: " + player.CritDamage +
            "\nCrit Damage: " + player.CritDamage;
    }

    public void DoPlayerLevelUp()
    {
        this.state = GameState.UpgradeMenu;
    }

    private List<UpgradePool> GetPossiblePools()
    {
        List<UpgradePool> pools = new List<UpgradePool>();
        pools.Add(ResourceManager.GetUpgradePool(ResourceManager.UpgradePoolIndex.Basic));
        pools.Add(ResourceManager.GetUpgradePool(ResourceManager.UpgradePoolIndex.Weapons));
        if (player.HasUpgrade(ResourceManager.UpgradeIndex.SnowballWeaponUpgrade))
        {
            pools.Add(ResourceManager.GetUpgradePool(ResourceManager.UpgradePoolIndex.Snowball));
        }
        if (player.HasUpgrade(ResourceManager.UpgradeIndex.PumpkinBombWeaponUpgrade))
        {
            pools.Add(ResourceManager.GetUpgradePool(ResourceManager.UpgradePoolIndex.Pumkin));
        }
        if (player.HasUpgrade(ResourceManager.UpgradeIndex.FireworkWeaponUpgrade))
        {
            pools.Add(ResourceManager.GetUpgradePool(ResourceManager.UpgradePoolIndex.Fireworks));
        }
        return pools;
    }

    public void ChooseSnowball()
    {
        StartGame(ResourceManager.UpgradeIndex.SnowballWeaponUpgrade);
    }

    public void ChoosePumpkin()
    {
        StartGame(ResourceManager.UpgradeIndex.PumpkinBombWeaponUpgrade);
    }

    public void ChooseFirework()
    {
        StartGame(ResourceManager.UpgradeIndex.FireworkWeaponUpgrade);
    }
    public void Retry()
    {
        gameOverPanel.gameObject.SetActive(false);
        EnemyManager.instance.Reset();
        Destroy(player.gameObject);
        time = 0.0f;
        state = GameState.Title;
    }

    public void StartGame(ResourceManager.UpgradeIndex weapon)
    {
        player = Instantiate<Player>(ResourceManager.playerPrefab, new Vector2(), Quaternion.identity);
        player.healthBar = healthBar;

        player.AddUpgrade(weapon);

        titlePanel.gameObject.SetActive(false);
        gamePanel.gameObject.SetActive(true);
        state = GameState.Normal;
    }
}
