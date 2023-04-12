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
        MainGame,
        Paused,
        UpgradeMenu,
        GameOver,
        LevelComplete,
        OptionsMenu,
        Cutscene
    }

    [SerializeField]
    public LevelData levelData;

    [SerializeField]
    public Camera cam;

    [SerializeField]
    private GameUIManager UIManager;

    [SerializeField]
    private InputActionReference displayStats, pauseGame, giveXP, playerDash, godMode, levelUpButton;

    [SerializeField]
    private InputAction slowToggle;

    [SerializeField]
    private InputAction completeLevel;

    [SerializeField]
    private UnityEngine.Rendering.Universal.Light2D globalLight;

    // Game state
    private GameState state = GameState.MainGame;

    // Time info
    private float time = 0f;
    private float dayLength = 180f; // 2.5 minutes
    [HideInInspector]
    public int currentDay = 1;
    [HideInInspector]
    public int currentHour = 12;
    private float baseTimeScale = 1f;
    private bool slowTime = false;
    public float slowTimeScale = 0.5f;

    // Tracking stats
    public int enemiesDefeated = 0;
    public int pickedUpGems = 0;

    // Flags
    private bool levelEnded = false;
    public bool doSpawns = true;
    private bool paused = false;

    // Cutscene manager
    private CutsceneManager cutscene;

    // Only a single GameManager should ever exist so we can always get it here
    [HideInInspector]
    public static GameManager instance;

    // The current session
    [HideInInspector]
    public static SessionManager session;

    // The player object
    private Player player;

    public Player Player
    {
        get { return player; }
    }

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
        set { state = value; }
    }

    void Start()
    {
        instance = this;

        // Checks if the session exists
        // No session means something went very wrong or the game is being run in the editor
        if (session == null)
        {
            if (DebugHelpers.DEBUG)
            {
                Debug.LogWarning("No session found - this does not cause any errors but you will not be able to exit the stage");
            }
            else
            {
                Debug.LogError("No session found");
            }

            // Initializes everything that would have been done at the start of the game and creates a player.
            ResourceManager.Init();
            PlayableCharacterData charData = ResourceManager.characters[1];
            player = GameObject.Instantiate<Player>(charData.prefab);
            player.inventory = new List<Item>();
            foreach (ItemDef i in charData.inventory)
            {
                Item item = i.GetItem();
                item.Level = 1;
                player.inventory.Add(item);
                if (i.GetType() == typeof(WeaponDef))
                {
                    player.AddWeapon(((WeaponDef)i).weaponPrefab);
                }
            }
            foreach (ItemDef i in charData.tutorialChestItems)
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
        else
        {
            // Gets a player instance from the current session and grabs the current level
            player = session.GetPlayerInstance();
            levelData = session.currentLevel;
        }

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
            UIManager.ToggleDebug();
        };

        giveXP.action.performed += (InputAction.CallbackContext callback) =>
        {
            player.AddXP(player.GetXpToNextLevel());
        };

        playerDash.action.performed += (InputAction.CallbackContext callback) =>
        {
            if (state == GameState.MainGame)
            {
                player.DoDash();
            }
        };

        godMode.action.performed += (InputAction.CallbackContext callback) =>
        {
            if (state == GameState.MainGame)
            {
                player.godMode = !player.godMode;
            }
        };

        if (DebugHelpers.DEBUG)
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

        // Initialize the UI manager
        UIManager.Init(player);
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
        // Updating the UI
        UIManager.UpdateUI(state, player, this);

        // Game Logic
        switch (state)
        {
            case GameState.GameOver: // GAME OVER
                player.canMove = false;
                break;
            case GameState.MainGame:
                player.canMove = true;
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

                // Moving the camera
                Vector3 camPos = new Vector3(Player.transform.position.x, Player.transform.position.y, cam.transform.position.z);
                cam.transform.position = camPos;

                // Pause screen
                if (paused)
                {
                    state = GameState.Paused;
                }

                // GameOver
                if (player.IsDead)
                {
                    state = GameState.GameOver;
                }
                break;
            case GameState.Paused: // PAUSE
                Time.timeScale = 0f;
                if (!paused)
                {
                    state = GameState.MainGame;
                }
                break;
            case GameState.UpgradeMenu: // UPGRADE MENU
                Time.timeScale = 0f;
                break;
            case GameState.LevelComplete: // LEVEL COMPLETE
                break;
            case GameState.OptionsMenu: // OPTIONS MENU
                break;
            case GameState.Cutscene: // CUTSCENE
                player.canMove = false;
                cutscene.Update();
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
        /*
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
        */
    }

    public void DoPlayerLevelUp()
    {
        if (player.waitingForLevels > 0)
        {
            this.state = GameState.UpgradeMenu;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowDamageNumbers(Toggle toggle)
    {
        UIManager.showDamageNumbers = toggle.isOn;
    }

    public void ShowWeaponIcons(Toggle toggle)
    {
        UIManager.showWeaponIcons = toggle.isOn;
    }

    /* 
     * 4 Seasons
     * Each season is 10 minutes
     * 5 Days per season
     * Each day is 2 minutes
    */
    public void UpdateDate()
    {
        currentDay = (int)Mathf.Floor(time / dayLength);
        currentHour = (int)((OffsetTime % dayLength) / (dayLength / 24));

        float currentHourFloat = ((OffsetTime % dayLength) / (dayLength / 24));
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

    // Called when the player picks up a gem. At the end of the stage this is added to the total from the stage
    public void pickupGems(int amount)
    {
        pickedUpGems += amount;
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
            Debug.LogWarning("No current session cannot return to title");
        }
    }

    public void ToTitle()
    {
        if (session != null)
        {
            ProjectileManager.Clean();
            EnemyManager.Clean();
            DropManager.Clean();
            session.ToTitle();
        }
        else
        {
            Debug.LogWarning("No current session cannot return to title");
        }
    }

    public void DisplayDamage(DamageInfo info)
    {
        UIManager.DisplayDamage(info);
    }

    public void DisplayHealing(float amount, StatsComponent receiver)
    {
        UIManager.DisplayHealing(amount, receiver);
    }

    public void ChestPickup(Chest c)
    {
        Debug.Log("Found chest");
        state = GameState.UpgradeMenu;
        UIManager.DoChestPickupUpgrade(c);
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

    public void CollectBossGem(BossGem gem)
    {
        if (state != GameState.Cutscene)
        {
            Debug.Log("Starting Cutscene");
            state = GameState.Cutscene;
            //EnemyManager.instance.KillAllAndStopSpawns();
            ProjectileManager.Clean();
            cutscene = new CutsceneManager();
            cutscene.DoCutscene(CutsceneManager.Cutscene.BossGemScene, player.gameObject, gem.gameObject, cam.gameObject);
        }
    }
}
