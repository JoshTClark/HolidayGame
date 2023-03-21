using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSceneManager : MonoBehaviour
{
    public enum SceneState
    {
        SelectSave,
        Title,
        CharacterSelect,
        UpgradeMenu,
        SaveEditor, 
        Options
    }

    [SerializeField]
    private Camera mainCam;

    [SerializeField]
    private CanvasRenderer titleScreenPanel, saveScreenPanel, charSelectPanel, upgradePanel, saveEditPanel, optionsPanel;

    public SceneState state = SceneState.SelectSave;

    public List<MetaUpgrade> upgrades = new List<MetaUpgrade>();
    private List<PlayableCharacterData> characters = new List<PlayableCharacterData>();
    private int currentSelectedCharacter = 0;

    [SerializeField]
    private TMP_Text charName, charInfo, costLabel, upgradeDesc, slot1Info, slot2Info, slot3Info, money;

    [SerializeField]
    private Image charImage;

    [SerializeField]
    private Button saveEditorButton, buyButton;

    [SerializeField]
    private TMP_InputField currencyInput;

    private GameData data;

    public static SaveSceneManager instance;

    private GameData[] saves = new GameData[3];

    [SerializeField]
    private InputAction debugToggle;

    private MetaUpgrade selected, hovered;

    private void Start()
    {
        ResourceManager.Init();
        instance = this;
        Cursor.visible = true;

        foreach (PlayableCharacterData i in ResourceManager.characters)
        {
            characters.Add(i);
        }

        if (!SaveManager.DoesFileExist(0))
        {
            Debug.Log("Save file 0 does not exist creating new file");
            GameData d = new GameData();
            d.id = 0;
            SaveManager.SaveFile(0, d);
        }
        if (!SaveManager.DoesFileExist(1))
        {
            Debug.Log("Save file 1 does not exist creating new file");
            GameData d = new GameData();
            d.id = 1;
            SaveManager.SaveFile(1, d);
        }
        if (!SaveManager.DoesFileExist(2))
        {
            Debug.Log("Save file 2 does not exist creating new file");
            GameData d = new GameData();
            d.id = 2;
            SaveManager.SaveFile(2, d);
        }

        if (DebugHelpers.DEBUG)
        {
            debugToggle.Enable();
        }
        debugToggle.performed += (InputAction.CallbackContext callback) =>
        {
            if (state != SceneState.SaveEditor && state != SceneState.SelectSave)
            {
                state = SceneState.SaveEditor;
                currencyInput.text = "" + data.currency;
            }
        };

        saves[0] = SaveManager.LoadFile(0);
        saves[1] = SaveManager.LoadFile(1);
        saves[2] = SaveManager.LoadFile(2);

        slot1Info.text = "<color=#00FF8B>" + saves[0].currency + " Gems";
        slot2Info.text = "<color=#00FF8B>" + saves[1].currency + " Gems";
        slot3Info.text = "<color=#00FF8B>" + saves[2].currency + " Gems";
    }



    private void Update()
    {
        switch (state)
        {
            case SceneState.SelectSave:
                titleScreenPanel.gameObject.SetActive(false);
                saveScreenPanel.gameObject.SetActive(true);
                charSelectPanel.gameObject.SetActive(false);
                upgradePanel.gameObject.SetActive(false);
                saveEditPanel.gameObject.SetActive(false);
                optionsPanel.gameObject.SetActive(false);
                break;
            case SceneState.Title:
                titleScreenPanel.gameObject.SetActive(true);
                saveScreenPanel.gameObject.SetActive(false);
                charSelectPanel.gameObject.SetActive(false);
                upgradePanel.gameObject.SetActive(false);
                saveEditPanel.gameObject.SetActive(false);
                optionsPanel.gameObject.SetActive(false);
                break;
            case SceneState.CharacterSelect:
                titleScreenPanel.gameObject.SetActive(false);
                saveScreenPanel.gameObject.SetActive(false);
                charSelectPanel.gameObject.SetActive(true);
                upgradePanel.gameObject.SetActive(false);
                saveEditPanel.gameObject.SetActive(false);
                optionsPanel.gameObject.SetActive(false);
                charName.text = characters[currentSelectedCharacter].characterName;
                charInfo.text = characters[currentSelectedCharacter].desc;
                charImage.sprite = characters[currentSelectedCharacter].characterImage;
                break;
            case SceneState.UpgradeMenu:
                titleScreenPanel.gameObject.SetActive(false);
                saveScreenPanel.gameObject.SetActive(false);
                charSelectPanel.gameObject.SetActive(false);
                upgradePanel.gameObject.SetActive(true);
                saveEditPanel.gameObject.SetActive(false);
                optionsPanel.gameObject.SetActive(false);
                if (selected)
                {
                    upgradeDesc.text = selected.upgradeDesc;
                    costLabel.text = selected.GetCost() + "";
                    if (data.currency >= selected.GetCost())
                    {
                        buyButton.gameObject.GetComponent<Image>().color = new Color(0.3f, 1f, 0.5f);
                        buyButton.enabled = true;
                    }
                    else
                    {
                        buyButton.gameObject.GetComponent<Image>().color = new Color(0.3f, 0.5f, 0.3f);
                        buyButton.enabled = false;
                    }
                }
                if (hovered)
                {
                    upgradeDesc.text = hovered.upgradeDesc;
                    costLabel.text = hovered.GetCost() + "";
                    if (data.currency >= hovered.GetCost())
                    {
                        buyButton.gameObject.GetComponent<Image>().color = new Color(0.3f, 1f, 0.5f);
                        buyButton.enabled = true;
                    }
                    else
                    {
                        buyButton.gameObject.GetComponent<Image>().color = new Color(0.3f, 0.5f, 0.3f);
                        buyButton.enabled = false;
                    }
                }
                money.text = data.currency + " Gems";
                break;
            case SceneState.SaveEditor:
                titleScreenPanel.gameObject.SetActive(false);
                saveScreenPanel.gameObject.SetActive(false);
                charSelectPanel.gameObject.SetActive(false);
                upgradePanel.gameObject.SetActive(false);
                saveEditPanel.gameObject.SetActive(true);
                optionsPanel.gameObject.SetActive(false);
                break;
            case SceneState.Options:
                titleScreenPanel.gameObject.SetActive(false);
                saveScreenPanel.gameObject.SetActive(false);
                charSelectPanel.gameObject.SetActive(false);
                upgradePanel.gameObject.SetActive(false);
                saveEditPanel.gameObject.SetActive(false);
                optionsPanel.gameObject.SetActive(true);
                break;
        }

        mainCam.transform.position = new Vector3(mainCam.transform.position.x + 1 * Time.deltaTime, mainCam.transform.position.y + 1 * Time.deltaTime, -10);
        if (mainCam.transform.position.x >= 50 && mainCam.transform.position.y >= 50)
        {
            mainCam.transform.position = new Vector3(-50, -50, -10);
        }

    }

    public void ToTitleScreen(int slot)
    {
        GameData d = SaveManager.LoadFile(slot);
        if (d != null)
        {
            data = d;
            data.init();
            state = SceneState.Title;
        }
    }

    public void ToTitleScreen()
    {
        state = SceneState.Title;
    }

    public void ToUpgradeScreen()
    {
        state = SceneState.UpgradeMenu;
        foreach (MetaUpgrade u in upgrades)
        {
            u.level = data.GetUpgradeLevel(u.id);
        }
    }

    public void DeleteButtonClick(int slot)
    {
        GameData data = SaveManager.LoadFile(slot);
        if (data != null)
        {
            SaveManager.Delete(slot);
            SaveManager.SaveFile(slot, new GameData());
        }
    }

    public void ToCharacterSelect()
    {
        state = SceneState.CharacterSelect;
    }

    public void StartGame()
    {
        SessionManager session = new SessionManager();
        SessionManager.data = data;
        session.SetChosenCharacter(characters[currentSelectedCharacter]);
        session.GenerateMap(6, 5, 20, 3);
        MapManager.session = session;
        SceneManager.LoadSceneAsync("MapScene");
    }

    public void CharButtonRight()
    {
        currentSelectedCharacter++;

        if (currentSelectedCharacter >= characters.Count)
        {
            currentSelectedCharacter = 0;
        }
    }

    public void CharButtonLeft()
    {
        currentSelectedCharacter--;

        if (currentSelectedCharacter < 0)
        {
            currentSelectedCharacter = characters.Count - 1;
        }
    }

    public void SaveEditorButton()
    {
        data.currency = int.Parse(currencyInput.text);
        SaveManager.SaveFile(data.id, data);
        ToTitleScreen();
    }

    public void BuyButtonClick()
    {
        if (selected)
        {
            if (data.currency >= selected.GetCost())
            {
                data.currency -= selected.GetCost();
                selected.level++;
                data.SetUpgrades(upgrades);
                SaveManager.SaveFile(data.id, data);
            }
        }
    }

    public void MetaUpgradeClick(MetaUpgrade upgrade)
    {
        selected = upgrade;
    }

    public void SetHovered(MetaUpgrade upgrade)
    {
        hovered = upgrade;
    }

    public void NotHovered()
    {
        hovered = null;
    }

    public void ToOptions() 
    {
        state = SceneState.Options;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
