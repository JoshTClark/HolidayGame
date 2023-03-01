using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public int slotNum;
    public Button selectSave, deleteSave;
    public TMP_Text infoLabel;
    public SaveSceneManager manager;

    void Start()
    {
        if (!SaveManager.DoesFileExist(slotNum))
        {
            SaveManager.SaveFile(slotNum, new GameData());
        }

        selectSave.onClick.AddListener(() =>
        {
            manager.ToTitleScreen(slotNum);
        });

        deleteSave.onClick.AddListener(() =>
        {
            manager.DeleteButtonClick(slotNum);
        });
    }
}
