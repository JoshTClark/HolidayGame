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
        CharacterSelect
    }

    [SerializeField]
    private Camera mainCam;

    [SerializeField]
    private CanvasRenderer titleScreenPanel, saveScreenPanel, charSelectPanel;

    public SceneState state = SceneState.SelectSave;

    private List<PlayableCharacterData> characters = new List<PlayableCharacterData>();
    private int currentSelectedCharacter = 0;

    [SerializeField]
    private TMP_Text charName, charInfo;

    [SerializeField]
    private Image charImage;

    public static SaveSceneManager instance;

    private void Start()
    {
        ResourceManager.Init();
        instance = this;

        foreach (PlayableCharacterData i in ResourceManager.characters)
        {
            characters.Add(i);
        }

        FileInfo[] info = SaveManager.LoadAllSaves();
    }

    private void Update()
    {
        switch (state)
        {
            case SceneState.SelectSave:
                titleScreenPanel.gameObject.SetActive(false);
                saveScreenPanel.gameObject.SetActive(true);
                charSelectPanel.gameObject.SetActive(false);
                break;
            case SceneState.Title:
                titleScreenPanel.gameObject.SetActive(true);
                saveScreenPanel.gameObject.SetActive(false);
                charSelectPanel.gameObject.SetActive(false);
                break;
            case SceneState.CharacterSelect:
                titleScreenPanel.gameObject.SetActive(false);
                saveScreenPanel.gameObject.SetActive(false);
                charSelectPanel.gameObject.SetActive(true);
                charName.text = characters[currentSelectedCharacter].characterName;
                charInfo.text = characters[currentSelectedCharacter].desc;
                charImage.sprite = characters[currentSelectedCharacter].characterImage;
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
        GameData data = SaveManager.LoadFile(slot);
        if (data != null)
        {
            state = SceneState.Title;
        }
    }

    public void ToTitleScreen()
    {
        state = SceneState.Title;
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
