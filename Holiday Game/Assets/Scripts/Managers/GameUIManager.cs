using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static GameManager;

public class GameUIManager : MonoBehaviour
{
    // The canvas containing all the UI objects
    [SerializeField]
    private Canvas canvas;

    // Current camera
    [SerializeField]
    public Camera cam;

    // The different layouts for the UI
    [SerializeField]
    private UILayout layout;

    // All the text fields that need to be updated
    [SerializeField]
    private TMP_Text objectiveDisplay, numberEffect, gemsText;

    // The cursor
    [SerializeField]
    private Image cursor;

    // All the different panels
    [SerializeField]
    private CanvasRenderer pausedPanel, gamePanel, upgradePanel, gameOverPanel, effectsPanel, debugPanel, levelCompletedPanel, optionsMenu;

    // Various othe gameobjects/prefabs
    [SerializeField]
    public GameObject weaponIconPrefab, bossHealth, bossHealthMask;

    // Options checks
    public bool showDamageNumbers = true;
    public bool showWeaponIcons = true;

    // All the weapon icons
    public List<WeaponIcon> weaponIcons = new List<WeaponIcon>();

    // Stuff for doing a chest upgrade with the upgrade panel
    private bool doChestUpgrade = false;
    private Chest chest;

    /// <summary>
    /// Initializes the UI
    /// </summary>
    /// <param name="player"></param>
    public void Init(Player player)
    {
        // Iniitalize the debug panel
        debugPanel.GetComponent<DebugPanel>().Init();

        // Set cursor to be not visible, custom cursor will take its place
        Cursor.visible = false;

        // Set all panels but gamePanel to not active
        gamePanel.gameObject.SetActive(true);
        pausedPanel.gameObject.SetActive(false);
        upgradePanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);
        debugPanel.gameObject.SetActive(false);
        levelCompletedPanel.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// Toggles displaying the debug menu
    /// </summary>
    public void ToggleDebug()
    {
        debugPanel.gameObject.SetActive(!debugPanel.gameObject.activeSelf);
        Cursor.visible = debugPanel.gameObject.activeSelf;
    }

    /// <summary>
    /// Updates the UI based on the current state
    /// </summary>
    /// <param name="state"></param>
    public void UpdateUI(GameManager.GameState state, Player player, GameManager gameManager)
    {
        // Setting the cursor position
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 cursorPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.GetComponent<RectTransform>(), mousePos, cam, out cursorPos);
        cursor.rectTransform.anchoredPosition = new Vector3(cursorPos.x, cursorPos.y, 0.0f);
        cursor.transform.SetAsLastSibling();

        // Switch statement to check the state
        switch (state)
        {
            case GameState.GameOver:
                // Set all panels except for GameOver to not active
                gameOverPanel.gameObject.SetActive(true);
                gamePanel.gameObject.SetActive(false);
                pausedPanel.gameObject.SetActive(false);
                upgradePanel.gameObject.SetActive(false);
                levelCompletedPanel.gameObject.SetActive(false);
                optionsMenu.gameObject.SetActive(false);
                break;
            case GameState.Paused:
                // Set all panels except for Paused to not active
                pausedPanel.gameObject.SetActive(true);
                gameOverPanel.gameObject.SetActive(false);
                gamePanel.gameObject.SetActive(false);
                upgradePanel.gameObject.SetActive(false);
                levelCompletedPanel.gameObject.SetActive(false);
                optionsMenu.gameObject.SetActive(false);
                break;
            case GameState.OptionsMenu:
                // Set all panels except for OptionsMenu to not active
                optionsMenu.gameObject.SetActive(true);
                upgradePanel.gameObject.SetActive(false);
                pausedPanel.gameObject.SetActive(false);
                gameOverPanel.gameObject.SetActive(false);
                gamePanel.gameObject.SetActive(false);
                levelCompletedPanel.gameObject.SetActive(false);
                break;
            case GameState.LevelComplete:
                // Set all panels except for LevelComplete to not active
                levelCompletedPanel.gameObject.SetActive(true);
                upgradePanel.gameObject.SetActive(false);
                pausedPanel.gameObject.SetActive(false);
                gameOverPanel.gameObject.SetActive(false);
                gamePanel.gameObject.SetActive(false);
                optionsMenu.gameObject.SetActive(false);

                // Gems text display
                if (session != null)
                {
                    gemsText.text = "You gained " + session.difficulty * 5 + " Gems";
                }
                break;
            case GameState.UpgradeMenu:
                // Set all panels except for UpgradePanel to not active
                upgradePanel.gameObject.SetActive(true);
                pausedPanel.gameObject.SetActive(false);
                gameOverPanel.gameObject.SetActive(false);
                gamePanel.gameObject.SetActive(false);
                levelCompletedPanel.gameObject.SetActive(false);
                optionsMenu.gameObject.SetActive(false);

                // Setting up the upgrade panel and displaying it
                UpgradePanelManager upgradeManager = upgradePanel.GetComponent<UpgradePanelManager>();
                if (!upgradeManager.displaying)
                {
                    upgradeManager.player = player;
                    if (doChestUpgrade)
                    {
                        upgradeManager.SetUpgrades(chest);
                        upgradeManager.ShowOptions();
                    }
                    else
                    {
                        upgradeManager.SetUpgrades(4);
                        upgradeManager.ShowOptions();
                    }
                }
                if (upgradeManager.selected)
                {
                    if (doChestUpgrade)
                    {
                        upgradeManager.Clear();
                        gameManager.State = GameState.MainGame;
                        doChestUpgrade = false;
                        upgradePanel.gameObject.SetActive(false);
                        gamePanel.gameObject.SetActive(true);
                    }
                    else
                    {
                        player.waitingForLevels--;
                        upgradeManager.Clear();
                        if (player.waitingForLevels <= 0)
                        {
                            gameManager.State = GameState.MainGame;
                            upgradePanel.gameObject.SetActive(false);
                            gamePanel.gameObject.SetActive(true);
                        }
                        else
                        {
                            upgradeManager.player = player;
                            upgradeManager.SetUpgrades(4);
                            upgradeManager.ShowOptions();
                        }
                    }
                }
                break;
            case GameState.MainGame:
                // Set all panels except for UpgradePanel to not active
                gamePanel.gameObject.SetActive(true);
                upgradePanel.gameObject.SetActive(false);
                pausedPanel.gameObject.SetActive(false);
                gameOverPanel.gameObject.SetActive(false);
                levelCompletedPanel.gameObject.SetActive(false);
                optionsMenu.gameObject.SetActive(false);

                // Updating objective display
                //objectiveDisplay.rectTransform.anchorMin = new Vector2(objectiveDisplay.rectTransform.anchorMin.x, 0.94f);
                //objectiveDisplay.rectTransform.anchorMax = new Vector2(objectiveDisplay.rectTransform.anchorMax.x, 1f);

                // Objective display
                bossHealth.SetActive(false);
                if (gameManager.levelData.isBossLevel && EnemyManager.instance.boss && EnemyManager.instance.boss.MaxHp != 0)
                {
                    // If level is boss level show boss display else hide it
                    objectiveDisplay.text = "Defeat the boss";
                    bossHealth.SetActive(true);
                    bossHealthMask.GetComponent<RectTransform>().anchorMax = new Vector2(EnemyManager.instance.boss.CurrentHP / EnemyManager.instance.boss.MaxHp, bossHealthMask.GetComponent<RectTransform>().anchorMax.y);
                    //objectiveDisplay.rectTransform.anchorMin = new Vector2(objectiveDisplay.rectTransform.anchorMin.x, 0.89f);
                    //objectiveDisplay.rectTransform.anchorMax = new Vector2(objectiveDisplay.rectTransform.anchorMax.x, 0.95f);
                }
                else if (gameManager.levelData.daysToSurvive > 0 && gameManager.levelData.daysToSurvive >= gameManager.currentDay)
                {
                    // Display days left to survive
                    objectiveDisplay.text = "Day " + gameManager.currentDay + "/" + gameManager.levelData.daysToSurvive;
                }
                else if (gameManager.levelData.enemiesToDefeat > 0 && gameManager.levelData.enemiesToDefeat > gameManager.enemiesDefeated)
                {
                    // Display enemies remaining
                    objectiveDisplay.text = gameManager.enemiesDefeated + "/" + gameManager.levelData.enemiesToDefeat + " Enemies Defeated";
                }
                else
                {
                    // All objective complete
                    objectiveDisplay.text = "Find the exit";
                }

                // Updating player stat display depending on the layout
                layout.UpdateUI(player);

                // Weapon Icon Stuff
                // Removing icons
                foreach (WeaponIcon icon in weaponIcons)
                {
                    if (!player.HasWeapon(icon.weaponIndex))
                    {
                        icon.weaponIndex = ResourceManager.WeaponIndex.Null;
                    }
                    if (!showWeaponIcons)
                    {
                        icon.gameObject.SetActive(false);
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
                        GameObject icon = GameObject.Instantiate<GameObject>(weaponIconPrefab, gamePanel.transform);
                        icon.GetComponentInChildren<WeaponIcon>().weaponIndex = weapon.index;
                        icon.GetComponentInChildren<WeaponIcon>().sprite = weapon.icon;
                        float x = 320 - (14 * (weaponIcons.Count + 1)) - (weaponIcons.Count * 14);
                        icon.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, -160);
                        weaponIcons.Add(icon.GetComponentInChildren<WeaponIcon>());
                    }
                }
                break;
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
                effect.GetComponent<NumberEffect>().canvas = canvas;
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
        effect.GetComponent<NumberEffect>().canvas = canvas;
        effect.GetComponent<NumberEffect>().cam = cam;
    }

    public void DoChestPickupUpgrade(Chest c)
    {
        doChestUpgrade = true;
        chest = c;
    }

    /// <summary>
    /// Class used to store objects that will be changed in the same way but in different positions
    /// </summary>
    [System.Serializable]
    public class UILayout
    {
        [SerializeField]
        public GameObject holder;

        [SerializeField]
        public TMP_Text playerLevel, hpText, dashText, levelUpText;

        [SerializeField]
        public Image xpBar, hpBar, dashTimer;

        public void UpdateUI(Player player)
        {
            if (dashText)
            {
                dashText.text = player.currentDashes.ToString();
            }
            if (xpBar)
            {
                xpBar.GetComponent<RectTransform>().anchorMax = new Vector2(player.GetPercentToNextLevel(), xpBar.GetComponent<RectTransform>().anchorMax.y);
            }
            if (dashTimer)
            {
                dashTimer.GetComponent<RectTransform>().anchorMax = new Vector2(1 - (player.dashCooldownTimer / player.DashCooldown), dashTimer.GetComponent<RectTransform>().anchorMax.y);
            }
            if (playerLevel)
            {
                playerLevel.text = "Level: " + player.Level;
            }
            if (hpText)
            {
                hpText.text = player.CurrentHP.ToString("0") + "/" + player.MaxHp.ToString("0");
            }
            if (hpBar)
            {
                hpBar.rectTransform.anchorMax = new Vector2(player.GetPercentHealth(), hpBar.rectTransform.anchorMax.y);
            }
            if (levelUpText)
            {
                if (player.waitingForLevels > 0) { levelUpText.gameObject.SetActive(true); }
                else { levelUpText.gameObject.SetActive(false); }
            }
        }
    }
}
