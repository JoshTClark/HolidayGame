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
        NewSave,
        Title,
        CharacterSelect
    }

    [SerializeField]
    private Camera mainCam;

    [SerializeField]
    private CanvasRenderer saveSelectPanel, newSavePanel, selectSlotPanel, titleScreenPanel, saveScreenPanel, titleScreenSubPanel, charSelectPanel;

    [SerializeField]
    private Button createSaveButton;

    [SerializeField]
    private TMP_Text errorText;

    [SerializeField]
    private TMP_InputField nameField;

    [SerializeField]
    private SaveSelector selector;

    public SceneState state = SceneState.SelectSave;

    public static SaveSceneManager instance;

    private void Start()
    {
        ResourceManager.Init();
        instance = this;

        FileInfo[] info = SaveManager.LoadAllSaves();
        foreach (FileInfo i in info)
        {
            selector.AddSlot(SaveManager.LoadFile(i.Name.Replace(i.Extension, "")));
        }


        createSaveButton.onClick.AddListener(() =>
        {
            if (nameField.text != "")
            {
                FinalizeSave(nameField.text);
            }
            else
            {
                errorText.gameObject.SetActive(true);
            }
        });
    }

    private void Update()
    {
        switch (state)
        {
            case SceneState.SelectSave:
                saveSelectPanel.gameObject.SetActive(true);
                newSavePanel.gameObject.SetActive(false);
                errorText.gameObject.SetActive(false);
                titleScreenPanel.gameObject.SetActive(false);
                saveScreenPanel.gameObject.SetActive(true);
                break;
            case SceneState.NewSave:
                saveSelectPanel.gameObject.SetActive(false);
                newSavePanel.gameObject.SetActive(true);
                titleScreenPanel.gameObject.SetActive(false);
                saveScreenPanel.gameObject.SetActive(true);
                break;
            case SceneState.Title:
                saveSelectPanel.gameObject.SetActive(false);
                newSavePanel.gameObject.SetActive(false);
                titleScreenPanel.gameObject.SetActive(true);
                saveScreenPanel.gameObject.SetActive(false);
                titleScreenSubPanel.gameObject.SetActive(true);
                charSelectPanel.gameObject.SetActive(false);
                break;
            case SceneState.CharacterSelect:
                saveSelectPanel.gameObject.SetActive(false);
                newSavePanel.gameObject.SetActive(false);
                titleScreenPanel.gameObject.SetActive(true);
                saveScreenPanel.gameObject.SetActive(false);
                titleScreenSubPanel.gameObject.SetActive(false);
                charSelectPanel.gameObject.SetActive(true);
                break;
        }

        mainCam.transform.position = new Vector3(mainCam.transform.position.x + 1 * Time.deltaTime, mainCam.transform.position.y + 1 * Time.deltaTime, -10);
        if (mainCam.transform.position.x >= 50 && mainCam.transform.position.y >= 50)
        {
            mainCam.transform.position = new Vector3(-50, -50, -10);
        }

    }

    public void CreateNewSave()
    {
        state = SceneState.NewSave;
    }

    private void FinalizeSave(string name)
    {
        GameData data = new GameData();
        data.saveName = name;
        SaveManager.SaveFile(name, data);
        selector.AddSlot(data);
        state = SceneState.SelectSave;
    }

    public void ToTitleScreen()
    {
        GameData data = selector.GetSelected();
        if (data != null)
        {
            state = SceneState.Title;
        }
    }

    public void DeleteButtonClick()
    {
        GameData data = selector.GetSelected();
        if (data != null)
        {
            selector.DeleteSlot();
        }
    }

    public void ToCharacterSelect()
    {
        state = SceneState.CharacterSelect;
    }

    public void StartGame(PlayableCharacterData character)
    {
        SessionManager session = new SessionManager();
        session.SetChosenCharacter(character);
        session.GenerateMap(10, 9, 10, 4);
        MapManager.session = session;
        SceneManager.LoadSceneAsync("MapScene");
    }
}
