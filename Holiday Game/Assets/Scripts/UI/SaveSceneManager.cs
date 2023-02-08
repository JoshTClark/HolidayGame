using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSceneManager : MonoBehaviour
{
    public enum SceneState
    {
        SelectSave,
        Title,
        CharacterSelect,
        UpgradeMenu
    }

    [SerializeField]
    private Camera mainCam;

    [SerializeField]
    private CanvasRenderer titleScreenPanel, saveScreenPanel, charSelectPanel, upgradePanel;

    public SceneState state = SceneState.SelectSave;

    public List<MetaUpgrade> upgrades = new List<MetaUpgrade>();
    private List<PlayableCharacterData> characters = new List<PlayableCharacterData>();
    private int currentSelectedCharacter = 0;

    [SerializeField]
    private TMP_Text charName, charInfo, costLabel, upgradeDesc, slot1Info, slot2Info, slot3Info, money;

    [SerializeField]
    private Image charImage;

    private GameData data;

    public static SaveSceneManager instance;

    private GameData[] saves = new GameData[3];

    private void Start()
    {
        ResourceManager.Init();
        instance = this;

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

        saves[0] = SaveManager.LoadFile(0);
        saves[1] = SaveManager.LoadFile(1);
        saves[2] = SaveManager.LoadFile(2);

        slot1Info.text = "$" + saves[0].currency;
        slot2Info.text = "$" + saves[1].currency;
        slot3Info.text = "$" + saves[2].currency;
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
                break;
            case SceneState.Title:
                titleScreenPanel.gameObject.SetActive(true);
                saveScreenPanel.gameObject.SetActive(false);
                charSelectPanel.gameObject.SetActive(false);
                upgradePanel.gameObject.SetActive(false);
                break;
            case SceneState.CharacterSelect:
                titleScreenPanel.gameObject.SetActive(false);
                saveScreenPanel.gameObject.SetActive(false);
                charSelectPanel.gameObject.SetActive(true);
                upgradePanel.gameObject.SetActive(false);
                charName.text = characters[currentSelectedCharacter].characterName;
                charInfo.text = characters[currentSelectedCharacter].desc;
                charImage.sprite = characters[currentSelectedCharacter].characterImage;
                break;
            case SceneState.UpgradeMenu:
                titleScreenPanel.gameObject.SetActive(false);
                saveScreenPanel.gameObject.SetActive(false);
                charSelectPanel.gameObject.SetActive(false);
                upgradePanel.gameObject.SetActive(true);
                foreach (MetaUpgrade i in upgrades)
                {
                    if (i.gameObject.GetComponent<HoverButton>().isHover)
                    {
                        upgradeDesc.text = i.upgradeDesc;
                        costLabel.text = "$" + i.GetCost();
                        break;
                    }
                    if (i.selected)
                    {
                        upgradeDesc.text = i.upgradeDesc;
                        costLabel.text = "$" + i.GetCost();
                    }
                }
                money.text = "$" + data.currency;
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
        session.GenerateMap(10, 9, 10, 4);
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
}
