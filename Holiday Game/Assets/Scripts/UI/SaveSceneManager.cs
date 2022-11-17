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
    }

    [SerializeField]
    private CanvasRenderer saveSelectPanel, newSavePanel, selectSlotPanel;

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
                break;
            case SceneState.NewSave:
                saveSelectPanel.gameObject.SetActive(false);
                newSavePanel.gameObject.SetActive(true);
                break;
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

    public void PlayButtonClick()
    {
        GameData data = selector.GetSelected();
        if (data != null)
        {
            GameManager.data = data;
            SceneManager.LoadScene("TestScene");
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
}
