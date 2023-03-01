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
        GameOver,
        LevelComplete,
        OptionsMenu
    }

    [SerializeField]
    public LevelData levelData;

    [SerializeField]
    public Camera cam;

    [SerializeField]
    private Canvas ui;

    [SerializeField]
    private TMP_Text objectiveDisplay, playerStats, playerLevel, hpText, numberEffect, dashText, levelUpText, gemsText;

    [SerializeField]
    private Image xpBar, hpBar, dashTimer, cursor;

    [SerializeField]
    private CanvasRenderer playerStatsPanel, pausedPanel, gamePanel, upgradePanel, gameOverPanel, effectsPanel, debugPanel, levelCompletedPanel, optionsMenu;

    [SerializeField]
    private InputActionReference displayStats, pauseGame, giveXP, playerDash, godMode, levelUpButton;

    [SerializeField]
    private UnityEngine.Rendering.Universal.Light2D globalLight;

    [SerializeField]
    public float cornDamageDone, snowballDamageDone, arrowDamageDone, pumpkinDamageDone, fireworkDamageDone;

    [SerializeField]
    public float cornKills, snowballKills, arrowKills, pumpkinKills, fireworkKills;

    public GameObject weaponIconPrefab, bossHealth, bossHealthMask;

    private float baseTimeScale = 1f;
    private bool slowTime = false;
    public float slowTimeScale = 0.5f;

    public bool showDamageNumbers = true;
    private float time = 0f;
    private GameState state = GameState.Normal;
    private bool paused = false;
    private int upgradesToGive = 0;
    private float dayLength = 120f;
    public int currentDay = 1;
    public int currentHour = 12;
    private List<string> seasonsOrdered = new List<string>();
    public List<WeaponIcon> weaponIcons = new List<WeaponIcon>();
    public int enemiesDefeated = 0;
    private bool levelEnded = false;
    public bool doSpawns = true;

    [SerializeField]
    private InputAction slowToggle;

    [SerializeField]
    private InputAction completeLevel;

    public Player Player
    {
        get { return player; }
    }

    public Player player;

    public float GameTime
    {
        get { return time; }
    }

    public float OffsetTime
    {
        get { return time + (levelData.startHour * (dayLength / 24)); }
    }

    public GameState State
    {
        get { return state; }
    }

    // Only a single gamemanager should ever exist so we can always get it here
    [HideInInspector]
    public static GameManager instance;

    [HideInInspector]
    public static SessionManager session;

    public bool CAMERA_TEST = false;

    void Start()
    {
        instance = this;

        if (session == null)
        {
            ResourceManager.Init();
            Debug.Log("No session found");
            if (CAMERA_TEST)
            {
                doSpawns = false;
                player = GameObject.Instantiate<Player>(Resources.Load<Player>("Prefabs/KnightTest32x32"));
            }
            else
            {
                player = GameObject.Instantiate<Player>(ResourceManager.characters[0].prefab);
                player.inventory = new List<Item>();
                foreach (ItemDef i in ResourceManager.characters[0].inventory)
                {
                    Item item = i.GetItem();
                    item.Level = 1;
                    player.inventory.Add(item);
                    if (i.GetType() == typeof(WeaponDef))
                    {
                        player.AddWeapon(((WeaponDef)i).weaponPrefab);
                    }
                }
                foreach (ItemDef i in ResourceManager.characters[0].tutorialChestItems)
                {
                    Item item = i.GetItem();
                    item.Level = 1;
                    player.inventory.Add(item);
                    if (i.GetType() == typeof(WeaponDef))
                    {
                        player.AddWeapon(((WeaponDef)i).weaponPrefab);
                    }
                }
                foreach (ItemDef i in ResourceManager.characters[0].tutorialChestItems)
                {
                    Item item = i.GetItem();
                    item.Level = 1;
                    player.inventory.Add(item);
                    if (i.GetType() == typeof(WeaponDef))
                    {
                        player.AddWeapon(((WeaponDef)i).weaponPrefab);
                    }
                }
            }
        }
        else
        {
            player = session.GetPlayerInstance();
            levelData = session.currentLevel;
        }

        Cursor.visible = false;
        debugPanel.GetComponent<DebugPanel>().Init();
        ResourceManager.GetBuffDef(ResourceManager.BuffIndex.Burning);

        seasonsOrdered.Add("Fall");
        seasonsOrdered.Add("Winter");
        seasonsOrdered.Add("Spring");
        seasonsOrdered.Add("Summer");

        pauseGame.action.performed += (InputAction.CallbackContext callback) =>
        {
            paused = true;
        };

        levelUpButton.action.performed += (InputAction.CallbackContext callback) =>
        {
            DoPlayerLevelUp();
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

        godMode.action.performed += (InputAction.CallbackContext callback) =>
        {
            if (state == GameState.Normal)
            {
                player.godMode = !player.godMode;
            }
        };

        for (int i = 0; i < player.maxWeapons; i++)
        {
            GameObject icon = Instantiate<GameObject>(weaponIconPrefab, gamePanel.transform);
            float space = 0.08f * player.maxWeapons;

            icon.GetComponent<RectTransform>().anchorMin = new Vector2((1f - (0.08f * player.maxWeapons)) + (0.08f * i) + 0.04f, 0.05f);
            icon.GetComponent<RectTransform>().anchorMax = new Vector2((1f - (0.08f * player.maxWeapons)) + (0.08f * i) + 0.04f, 0.05f);
            weaponIcons.Add(icon.GetComponentInChildren<WeaponIcon>());
        }

        if (Constants.DEBUG)
        {
            slowToggle.Enable();
            completeLevel.Enable();
        }
        slowToggle.performed += (InputAction.CallbackContext callback) =>
        {
            slowTime = !slowTime;
        };
        completeLevel.performed += (InputAction.CallbackContext callback) =>
        {
            DoLevelEnd();
        };

        pausedPanel.gameObject.SetActive(false);
        upgradePanel.gameObject.SetActive(false);
        gamePanel.gameObject.SetActive(true);
        gameOverPanel.gameObject.SetActive(false);
        debugPanel.gameObject.SetActive(false);
        levelCompletedPanel.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(false);
    }

    public void OnEnable()
    {

    }

    public void OnDisable()
    {
        displayStats.action.Disable();
        pauseGame.action.Disable();
        giveXP.action.Disable();
        playerDash.action.Disable();
        godMode.action.Disable();
        levelUpButton.action.Disable();
        slowToggle.Disable();
        completeLevel.Disable();

    }

    void Update()
    {
        cursor.rectTransform.position = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0);
        cursor.transform.SetAsLastSibling();

        switch (state)
        {
            case GameState.GameOver:
                gamePanel.gameObject.SetActive(false);
                gameOverPanel.gameObject.SetActive(true);
                break;
            case GameState.Normal:
                if (!slowTime)
                {
                    Time.timeScale = baseTimeScale;
                }
                else
                {
                    Time.timeScale = slowTimeScale;
                }
                // Updating the timer and difficulty
                if (doSpawns)
                {
                    time += Time.deltaTime;
                }
                UpdateDate();

                // Updating displays
                objectiveDisplay.rectTransform.anchorMin = new Vector2(objectiveDisplay.rectTransform.anchorMin.x, 0.94f);
                objectiveDisplay.rectTransform.anchorMax = new Vector2(objectiveDisplay.rectTransform.anchorMax.x, 1f);
                bossHealth.SetActive(false);
                if (levelData.isBossLevel && EnemyManager.instance.boss && EnemyManager.instance.boss.MaxHp != 0)
                {
                    objectiveDisplay.text = "Defeat the boss";
                    bossHealth.SetActive(true);
                    bossHealthMask.GetComponent<RectTransform>().anchorMax = new Vector2(EnemyManager.instance.boss.CurrentHP / EnemyManager.instance.boss.MaxHp, bossHealthMask.GetComponent<RectTransform>().anchorMax.y);
                    objectiveDisplay.rectTransform.anchorMin = new Vector2(objectiveDisplay.rectTransform.anchorMin.x, 0.89f);
                    objectiveDisplay.rectTransform.anchorMax = new Vector2(objectiveDisplay.rectTransform.anchorMax.x, 0.95f);
                }
                else if (levelData.daysToSurvive > 0 && levelData.daysToSurvive >= currentDay)
                {
                    objectiveDisplay.text = "Day " + currentDay + "/" + levelData.daysToSurvive;
                }
                else if (levelData.enemiesToDefeat > 0 && levelData.enemiesToDefeat > enemiesDefeated)
                {
                    objectiveDisplay.text = enemiesDefeated + "/" + levelData.enemiesToDefeat + " Enemies Defeated";
                }
                else
                {
                    objectiveDisplay.text = "Find the exit";
                }
                dashText.text = player.currentDashes.ToString();
                xpBar.GetComponent<RectTransform>().anchorMax = new Vector2(player.GetPercentToNextLevel(), xpBar.GetComponent<RectTransform>().anchorMax.y);
                dashTimer.GetComponent<RectTransform>().anchorMax = new Vector2(1 - (player.dashCooldownTimer / player.DashCooldown), dashTimer.GetComponent<RectTransform>().anchorMax.y);
                playerLevel.text = "Level: " + player.Level;
                /*
                Debug.Log(player.CurrentHP + " current hp");
                Debug.Log(player.MaxHp + " max hp");
                Debug.Log(player.GetPercentHealth() + "% player health");
                Debug.Log(hpBar.rectTransform.anchorMin + " anchor min");
                Debug.Log(hpBar.rectTransform.anchorMax + " anchor max");
                */
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

                if (player.waitingForLevels > 0)
                {
                    levelUpText.gameObject.SetActive(true);
                }
                else { levelUpText.gameObject.SetActive(false); }

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
                pausedPanel.gameObject.SetActive(true);
                optionsMenu.gameObject.SetActive(false);
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
                    upgradeManager.SetUpgrades(4);
                    upgradeManager.ShowOptions(upgradesToGive);
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
                    }
                    else
                    {
                        upgradeManager.player = player;
                        upgradeManager.SetUpgrades(4);
                        upgradeManager.ShowOptions(upgradesToGive);
                    }
                }
                break;
            case GameState.LevelComplete:
                pausedPanel.gameObject.SetActive(false);
                upgradePanel.gameObject.SetActive(false);
                gamePanel.gameObject.SetActive(false);
                gameOverPanel.gameObject.SetActive(false);
                debugPanel.gameObject.SetActive(false);
                levelCompletedPanel.gameObject.SetActive(true);
                break;
            case GameState.OptionsMenu:
                pausedPanel.gameObject.SetActive(false);
                upgradePanel.gameObject.SetActive(false);
                gamePanel.gameObject.SetActive(false);
                gameOverPanel.gameObject.SetActive(false);
                debugPanel.gameObject.SetActive(false);
                levelCompletedPanel.gameObject.SetActive(false);
                optionsMenu.gameObject.SetActive(true);
                break;
        }
    }

    // Gives the player the desired weapon
    public void GivePlayerWeapon(ResourceManager.WeaponIndex index)
    {
        Weapon weapon = ResourceManager.GetWeapon(index);
        Player.AddWeapon(weapon);
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
            "\nTime Alive " + minutes + ":" + seconds;
    }

    public void PlayerPickupBossDrop(int upgrades)
    {
        upgradesToGive = upgrades;
        state = GameState.UpgradeMenu;
    }

    public void DoPlayerLevelUp()
    {
        if (player.waitingForLevels > 0)
        {
            this.state = GameState.UpgradeMenu;
            upgradesToGive = player.waitingForLevels;
            player.waitingForLevels = 0;
        }
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
        currentDay = (int)Mathf.Floor(time / dayLength) + 1;
        currentHour = (int)((OffsetTime % dayLength) / (dayLength / 24));

        float percentThroughDay = (time % dayLength) / dayLength;

        /*
        dayBar1.rectTransform.anchorMin = new Vector2(0f - percentThroughDay - 0.5f, dayBar1.rectTransform.anchorMin.y);
        dayBar1.rectTransform.anchorMax = new Vector2(1f - percentThroughDay - 0.5f, dayBar1.rectTransform.anchorMax.y);
        dayBar2.rectTransform.anchorMin = new Vector2(1f - percentThroughDay - 0.5f, dayBar2.rectTransform.anchorMin.y);
        dayBar2.rectTransform.anchorMax = new Vector2(2f - percentThroughDay - 0.5f, dayBar2.rectTransform.anchorMax.y);
        dayBar3.rectTransform.anchorMin = new Vector2(2f - percentThroughDay - 0.5f, dayBar3.rectTransform.anchorMin.y);
        dayBar3.rectTransform.anchorMax = new Vector2(3f - percentThroughDay - 0.5f, dayBar3.rectTransform.anchorMax.y);
        */

        float currentHourFloat = ((OffsetTime % dayLength) / (dayLength / 24));
        //globalLight.intensity = 0.1f;
        globalLight.intensity = Mathf.Clamp(1f - Mathf.Pow(Mathf.Abs(currentHourFloat - 12) / 12f, 3f / 4f) + (0.2f * (0.9f - Mathf.Abs(currentHourFloat - 12) / 12f)), 0f, 1f);
    }

    public bool IsDayLight()
    {
        if (Mathf.RoundToInt((OffsetTime - (dayLength / 4)) / dayLength) % 2 == 0)
        {
            return true;
        }

        return false;
    }

    //Currency Management
    public void GainGold(int amount)
    {
        SessionManager.money += amount;
    }

    // Called to end the level
    public void EndLevel()
    {
        if (!levelEnded)
        {
            if (levelData.daysToSurvive > 0 || levelData.enemiesToDefeat > 0)
            {
                if (levelData.enemiesToDefeat <= enemiesDefeated && levelData.enemiesToDefeat > 0)
                {
                    Debug.Log("End Level - Defeated Enemies");
                    levelEnded = true;
                    DoLevelEnd();
                }

                if (levelData.daysToSurvive <= currentDay && levelData.daysToSurvive > 0)
                {
                    Debug.Log("End Level - Survived");
                    levelEnded = true;
                    DoLevelEnd();
                }
            }
            else
            {
                Debug.Log("End Level - Found Exit");
                levelEnded = true;
                DoLevelEnd();
            }
        }
    }

    private void DoLevelEnd()
    {
        if (session != null)
        {
            gemsText.text = "You gained " + session.difficulty * 5 + " Gems";
        }
        state = GameState.LevelComplete;
        player.gameObject.SetActive(false);
        //EnemyManager.instance.KillAllAndStopSpawns();
    }

    public void ToMap()
    {
        if (session != null)
        {
            ProjectileManager.Clean();
            EnemyManager.Clean();
            DropManager.Clean();
            session.LevelComplete(session.difficulty * 5, player);
        }
        else
        {
            Debug.Log("No current session cannot go to map");
        }
    }

    public void ToTitle()
    {
        ProjectileManager.Clean();
        EnemyManager.Clean();
        DropManager.Clean();
        session.ToTitle();
    }

    public void ChestPickup(Chest c)
    {
        Debug.Log("Found chest");
        state = GameState.UpgradeMenu;
        upgradesToGive = 1;
        upgradePanel.GetComponent<UpgradePanelManager>().chest = c;
    }

    /// <summary>
    /// Called when the continue button is clicked in the paused menu
    /// </summary>
    public void ContinueButton()
    {
        paused = false;
    }

    /// <summary>
    /// Called when the return button is clicked in the options menu
    /// </summary>
    public void OptionsBackButton()
    {
        state = GameState.Paused;
    }

    /// <summary>
    /// Called when the options button is clicked in the paused menu
    /// </summary>
    public void OptionsButton()
    {
        state = GameState.OptionsMenu;
    }
}
