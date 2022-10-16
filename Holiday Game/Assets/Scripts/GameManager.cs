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
    private TMP_Text timerDisplay, playerStats, playerLevel, hpText, numberEffect;

    [SerializeField]
    private Image xpBar, hpBar;

    [SerializeField]
    private CanvasRenderer playerStatsPanel, pausedPanel, gamePanel, upgradePanel, titlePanel, gameOverPanel, effectsPanel;

    [SerializeField]
    private float timeToDifficultyIncrease;

    [SerializeField]
    private InputActionReference displayStats, pauseGame, giveXP;

    /*
    [SerializeField]
    private HealthBar healthBar;
    */

    public bool showDamageNumbers = true;
    private float time = 0.0f;
    private float currentDifficulty = 1;
    private GameState state = GameState.Title;
    private bool paused = false;
    private int month, day, hour;
    private float secondToHourRation = 1 / 1;

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
                CalculateDate();

                // Updating displays
                if (hour > 12)
                {
                    timerDisplay.text = (hour - 12) + " PM";
                }
                else
                {
                    timerDisplay.text = hour + " AM";
                }
                xpBar.GetComponent<RectTransform>().anchorMax = new Vector2(player.GetPercentToNextLevel(), xpBar.GetComponent<RectTransform>().anchorMax.y);
                playerLevel.text = "Level: " + player.Level;
                hpBar.rectTransform.anchorMax = new Vector2(player.GetPercentHealth(), hpBar.rectTransform.anchorMax.y);
                hpText.text = player.CurrentHP + "/" + player.MaxHp;

                // Moving the camera
                Vector3 camPos = new Vector3(Player.transform.position.x, Player.transform.position.y, cam.transform.position.z);
                cam.transform.position = camPos;

                // Pause screen
                if (paused)
                {
                    DisplayStats();
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
        string minutes = Mathf.Floor(time / 60).ToString();
        string seconds = (time % 60).ToString("00");
        playerStats.text =
            "Max HP: " + player.MaxHp +
            "\nSpeed: " + player.Speed +
            "\nDamage: " + player.Damage +
            "\nAttack Speed:" + player.AttackSpeed +
            "\nArmor: " + player.Armor +
            "\nRegen: " + player.Regen +
            "\nCrit Chance: " + (player.CritChance * 100) + "% " +
            "\nCrit Damage: " + player.CritDamage +
            "\nTime Alive " + minutes + ":" + seconds +
            "\nGame Difficulty: " + currentDifficulty.ToString();
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
        //player.healthBar = healthBar;

        player.AddUpgrade(weapon);

        titlePanel.gameObject.SetActive(false);
        gamePanel.gameObject.SetActive(true);
        state = GameState.Normal;
    }

    public void DisplayDamage(DamageInfo info)
    {
        if (info.receiver && showDamageNumbers)
        {
            if (info.receiver.GetType() != typeof(Player))
            {
                TMP_Text effect = Instantiate<TMP_Text>(numberEffect, effectsPanel.gameObject.transform);
                effect.text = info.damage.ToString();
                effect.color = info.GetColor();
                if (info.damageColor == DamageInfo.DamageColor.Crit)
                {
                    effect.fontStyle = FontStyles.Bold;
                }
                effect.GetComponent<NumberEffect>().spawnPosition = info.receiver.gameObject.transform.position;
                effect.GetComponent<NumberEffect>().canvas = ui;
                effect.GetComponent<NumberEffect>().cam = cam;
            }
        }
    }

    public void DisplayHealing(float amount, StatsComponent receiver)
    {
        TMP_Text effect = Instantiate<TMP_Text>(numberEffect, effectsPanel.gameObject.transform);
        effect.text = amount.ToString();
        effect.color = new Color(0f, 1f, 0f, 1f);
        effect.GetComponent<NumberEffect>().spawnPosition = receiver.gameObject.transform.position;
        effect.GetComponent<NumberEffect>().canvas = ui;
        effect.GetComponent<NumberEffect>().cam = cam;
    }

    public void ShowDamageNumbers(bool show)
    {
        showDamageNumbers = show;
    }

    /* 
     * Calculates the date of the in game timer
     * 1 second = 1 hour
     * 24 seconds = 1 Day
     * 
     * 744 hours in January - Starts at 0
     * 672 hours in Febuary - Starts at 744
     * 744 hours in March - Starts at 1416
     * 720 hours in April - Starts at 2160
     * 744 hours in May - Starts at 2880
     * 720 hours in June - Starts at 3624
     * 744 hours in July - Starts at 4344
     * 744 hours in August - Starts at 5088
     * 720 hours in September - Starts at 5832
     * 744 hours in October - Starts at 6552
     * 720 hours in November - Starts at 7296
     * 744 hours in December - Starts at 8016
     * Ends at 8760
    */
    public void CalculateDate()
    {

        int totalHours = (int)Mathf.Floor(time * secondToHourRation);
        hour = (totalHours % 24) + 1;
    }
}
