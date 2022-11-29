using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
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
    private TMP_Text timerDisplay, playerStats, playerLevel, hpText, numberEffect, dashText;

    [SerializeField]
    private Image xpBar, hpBar, dashTimer, dayBar1, dayBar2, dayBar3, cursor;

    [SerializeField]
    private CanvasRenderer playerStatsPanel, pausedPanel, gamePanel, upgradePanel, titlePanel, gameOverPanel, effectsPanel, debugPanel;

    [SerializeField]
    private float timeToDifficultyIncrease;

    [SerializeField]
    private InputActionReference displayStats, pauseGame, giveXP, playerDash;

    [SerializeField]
    private UnityEngine.Rendering.Universal.Light2D globalLight;

    [SerializeField]
    public List<WeaponIcon> weaponIcons;

    [SerializeField]
    public LevelData level;

    [SerializeField]
    public float cornDamageDone, snowballDamageDone, arrowDamageDone, pumpkinDamageDone, fireworkDamageDone;

    [SerializeField]
    public float cornKills, snowballKills, arrowKills, pumpkinKills, fireworkKills;

    public bool showDamageNumbers = true;
    private float time = 0f;
    private float enemyLevel = 1;
    private GameState state = GameState.Title;
    private bool paused = false;
    private int garunteedWeaponLevel = 5;
    private bool doSpecialUpgrade = false;
    private int upgradesToGive = 0;
    private float dayLength = 120f;
    public int currentDay = 1;
    public int currentSeason = 0;
    public int currentHour = 12;
    private List<string> seasonsOrdered = new List<string>();

    public Player Player
    {
        get { return player; }
    }

    private Player player;

    public float CurrentDifficulty
    {
        get { return enemyLevel; }
    }

    public float GameTime
    {
        get { return time; }
    }

    public GameState State
    {
        get { return state; }
    }

    // Only a single gamemanager should ever exist so we can always get it here
    public static GameManager instance;

    // The save game data to use
    public static GameData data;

    void Start()
    {
        instance = this;
        //Cursor.SetCursor(cursorTexture, new Vector2(15, 15), CursorMode.Auto);    
        Cursor.visible = false;
        ResourceManager.Init();
        debugPanel.GetComponent<DebugPanel>().Init();
        ResourceManager.GetBuffDef(ResourceManager.BuffIndex.Burning);

        seasonsOrdered.Add("Fall");
        seasonsOrdered.Add("Winter");
        seasonsOrdered.Add("Spring");
        seasonsOrdered.Add("Summer");

        pauseGame.action.performed += (InputAction.CallbackContext callback) =>
        {
            paused = !paused;
        };

        displayStats.action.performed += (InputAction.CallbackContext callback) =>
        {
            debugPanel.gameObject.SetActive(!debugPanel.gameObject.activeSelf);
        };

        giveXP.action.performed += (InputAction.CallbackContext callback) =>
        {
            player.AddXP(player.GetXpToNextLevel() - player.XP + 1);
        };

        playerDash.action.performed += (InputAction.CallbackContext callback) =>
        {
            if (state == GameState.Normal)
            {
                player.DoDash();
            }
        };

        pausedPanel.gameObject.SetActive(false);
        upgradePanel.gameObject.SetActive(false);
        gamePanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);
        titlePanel.gameObject.SetActive(true);
        debugPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        cursor.rectTransform.position = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0);
        cursor.transform.SetAsLastSibling();

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
                Time.timeScale = 1f;
                // Updating the timer and difficulty
                time += Time.deltaTime;
                enemyLevel = Mathf.Floor(1 + (time / timeToDifficultyIncrease));
                UpdateDate();

                // Updating displays
                timerDisplay.text = seasonsOrdered[currentSeason] + ": Day " + currentDay;
                dashText.text = player.currentDashes.ToString();
                xpBar.GetComponent<RectTransform>().anchorMax = new Vector2(player.GetPercentToNextLevel(), xpBar.GetComponent<RectTransform>().anchorMax.y);
                dashTimer.GetComponent<RectTransform>().anchorMax = new Vector2(1 - (player.dashCooldownTimer / player.DashCooldown), dashTimer.GetComponent<RectTransform>().anchorMax.y);
                playerLevel.text = "Level: " + player.Level;
                hpBar.rectTransform.anchorMax = new Vector2(player.GetPercentHealth(), hpBar.rectTransform.anchorMax.y);
                hpText.text = player.CurrentHP.ToString("0") + "/" + player.MaxHp.ToString("0");

                // Moving the camera
                Vector3 camPos = new Vector3(Player.transform.position.x, Player.transform.position.y, cam.transform.position.z);
                cam.transform.position = camPos;

                // Removing icons
                foreach (WeaponIcon icon in weaponIcons)
                {
                    if (!player.HasWeapon(icon.weaponIndex))
                    {
                        icon.weaponIndex = ResourceManager.WeaponIndex.Null;
                        //icon.gameObject.SetActive(false);
                    }
                }

                // Adding new icons
                foreach (Weapon weapon in player.weapons)
                {
                    bool hasIcon = false;
                    foreach (WeaponIcon icon in weaponIcons)
                    {
                        if (weapon.index == icon.weaponIndex)
                        {
                            hasIcon = true;
                        }
                    }

                    if (!hasIcon)
                    {
                        int i = 0;
                        while (weaponIcons[i].weaponIndex != ResourceManager.WeaponIndex.Null)
                        {
                            i++;
                        }
                        weaponIcons[i].weaponIndex = weapon.index;
                        weaponIcons[i].sprite = weapon.icon;
                        weaponIcons[i].gameObject.SetActive(true);
                    }
                }

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
                Time.timeScale = 0f;
                if (!paused)
                {
                    state = GameState.Normal;
                    pausedPanel.gameObject.SetActive(false);
                }
                break;
            case GameState.UpgradeMenu:
                Time.timeScale = 0f;
                upgradePanel.gameObject.SetActive(true);
                gamePanel.gameObject.SetActive(false);
                UpgradePanelManager upgradeManager = upgradePanel.GetComponent<UpgradePanelManager>();
                if (!upgradeManager.displaying)
                {
                    upgradeManager.player = player;
                    upgradeManager.SetUpgradesByPools(GetPossiblePools(false), 4);
                    bool check = false;
                    if ((player.Level - (upgradesToGive - 1)) % garunteedWeaponLevel == 0)
                    {
                        check = true;
                    }
                    upgradeManager.ShowOptions(upgradesToGive, check);
                }
                if (upgradeManager.selected)
                {
                    upgradesToGive--;
                    upgradeManager.Clear();
                    if (upgradesToGive <= 0)
                    {
                        state = GameState.Normal;
                        upgradePanel.gameObject.SetActive(false);
                        gamePanel.gameObject.SetActive(true);
                        doSpecialUpgrade = false;
                    }
                    else
                    {
                        upgradeManager.player = player;
                        upgradeManager.SetUpgradesByPools(GetPossiblePools(false), 4);
                        bool check = false;
                        if ((player.Level - (upgradesToGive - 1)) % garunteedWeaponLevel == 0)
                        {
                            check = true;
                        }
                        upgradeManager.ShowOptions(upgradesToGive, check);
                    }
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
        string minutes = Mathf.Floor(time / 60f).ToString();
        string seconds = (time % 60f).ToString("00");
        playerStats.text =
            "Max HP: " + player.MaxHp +
            "\nSpeed: " + player.Speed +
            "\nDamage: " + player.Damage +
            "\nAttack Speed:" + player.AttackSpeed + "x" +
            "\nArmor: " + player.Armor +
            "\nRegen: " + player.Regen +
            "\nCrit Chance: " + (player.CritChance * 100) + "% " +
            "\nCrit Damage: " + player.CritDamage + "x" +
            "\nTime Alive " + minutes + ":" + seconds +
            "\nGame Difficulty: " + enemyLevel.ToString();
    }

    public void PlayerPickupBossDrop(int upgrades)
    {
        upgradesToGive = upgrades;
        doSpecialUpgrade = true;
        state = GameState.UpgradeMenu;
    }

    public void DoPlayerLevelUp(int levels)
    {
        this.state = GameState.UpgradeMenu;
        upgradesToGive = levels;
    }

    public List<UpgradePool> GetPossiblePools(bool ignoreWeapons)
    {
        List<UpgradePool> pools = new List<UpgradePool>();
        if (doSpecialUpgrade)
        {
            pools.Add(ResourceManager.GetUpgradePool(ResourceManager.UpgradePoolIndex.SpecialUpgrades));
        }
        else if ((player.Level - (upgradesToGive - 1)) % garunteedWeaponLevel == 0 && !ignoreWeapons)
        {
            pools.Add(ResourceManager.GetUpgradePool(ResourceManager.UpgradePoolIndex.Weapons));
        }
        else
        {
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
            if (player.HasUpgrade(ResourceManager.UpgradeIndex.CupidArrowWeaponUpgrade))
            {
                pools.Add(ResourceManager.GetUpgradePool(ResourceManager.UpgradePoolIndex.CupidArrow));
            }
            if (player.HasUpgrade(ResourceManager.UpgradeIndex.CandyCornWeaponUpgrade))
            {
                pools.Add(ResourceManager.GetUpgradePool(ResourceManager.UpgradePoolIndex.CandyCorn));
            }
        }
        return pools;
    }

    public void Retry()
    {
        ProjectileManager.Clean();
        DropManager.Clean();
        gameOverPanel.gameObject.SetActive(false);
        EnemyManager.instance.Reset();
        Destroy(player.gameObject);
        time = 0.0f;
        upgradesToGive = 0;
        state = GameState.Title;
    }

    public void StartGame(ResourceManager.UpgradeIndex weapon)
    {
        player = Instantiate<Player>(ResourceManager.playerPrefab, new Vector2(), Quaternion.identity);
        //player.healthBar = healthBar;

        player.AddUpgrade(weapon);

        ProjectileManager.Clean();
        level.Clean();
        level.CreateLevel();
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
                effect.text = info.damage.ToString("0.0");
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
        effect.text = amount.ToString("0.0");
        effect.color = new Color(0f, 1f, 0f, 1f);
        effect.GetComponent<NumberEffect>().spawnPosition = receiver.gameObject.transform.position;
        effect.GetComponent<NumberEffect>().canvas = ui;
        effect.GetComponent<NumberEffect>().cam = cam;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowDamageNumbers(bool show)
    {
        showDamageNumbers = show;
    }

    /* 
     * 4 Seasons
     * Each season is 10 minutes
     * 5 Days per season
     * Each day is 2 minutes
    */
    public void UpdateDate()
    {
        currentSeason = (int)Mathf.Floor(time / (dayLength * 5));
        currentDay = (int)Mathf.Floor((time - ((currentSeason - 1) * 5)) / dayLength) + 1;
        currentHour = (int)((time % dayLength) / (dayLength / 24));

        float percentThroughDay = (time % dayLength) / dayLength;

        dayBar1.rectTransform.anchorMin = new Vector2(0f - percentThroughDay - 0.5f, dayBar1.rectTransform.anchorMin.y);
        dayBar1.rectTransform.anchorMax = new Vector2(1f - percentThroughDay - 0.5f, dayBar1.rectTransform.anchorMax.y);
        dayBar2.rectTransform.anchorMin = new Vector2(1f - percentThroughDay - 0.5f, dayBar2.rectTransform.anchorMin.y);
        dayBar2.rectTransform.anchorMax = new Vector2(2f - percentThroughDay - 0.5f, dayBar2.rectTransform.anchorMax.y);
        dayBar3.rectTransform.anchorMin = new Vector2(2f - percentThroughDay - 0.5f, dayBar3.rectTransform.anchorMin.y);
        dayBar3.rectTransform.anchorMax = new Vector2(3f - percentThroughDay - 0.5f, dayBar3.rectTransform.anchorMax.y);

        float currentHourFloat = ((time % dayLength) / (dayLength / 24));
        //globalLight.intensity = 0.1f;
        globalLight.intensity = Mathf.Clamp(1f - Mathf.Pow(Mathf.Abs(currentHourFloat - 12) / 12f, 3f/4f) + (0.2f * (0.9f - Mathf.Abs(currentHourFloat - 12) / 12f)), 0f, 1f);
    }

    public bool IsDayLight()
    {
        if (Mathf.RoundToInt((time - (dayLength / 4)) / dayLength) % 2 == 0)
        {
            return true;
        }

        return false;
    }
}
