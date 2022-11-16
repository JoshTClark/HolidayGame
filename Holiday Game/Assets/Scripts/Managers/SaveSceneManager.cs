using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSceneManager : MonoBehaviour
{
    public enum SceneState
    {
        SelectSave,
        NewSave
    }

    [SerializeField]
    private CanvasRenderer saveSelectPanel, newSavePanel; 

    public SceneState state = SceneState.SelectSave;

    public static SaveSceneManager instance;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        switch (state)
        {
            case SceneState.SelectSave:
                saveSelectPanel.gameObject.SetActive(true);
                newSavePanel.gameObject.SetActive(false);
                break;
            case SceneState.NewSave:
                saveSelectPanel.gameObject.SetActive(false);
                newSavePanel.gameObject.SetActive(true);
                break;
        }
    }

    public void CreateNewSave(int slot)
    {
        state = SceneState.NewSave;
    }

    public void FinalizeSave()
    {
        //SaveManager.SaveFile("save" + slot, new GameData());
    }
}
