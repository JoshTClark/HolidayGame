using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveFileButton : HoverButton
{
    public int saveSlot;
    public void OnPress()
    {
        if (SaveManager.DoesFileExist("save" + saveSlot))
        {
            SceneManager.LoadScene("TestScene");
        }
        else
        {
            SaveSceneManager.instance.CreateNewSave(saveSlot);
        }
    }
}
