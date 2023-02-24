using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    public Chest chest;

    public void Start()
    {
        if (GameManager.session != null) {
            for (int i = 0; i < GameManager.session.playerData.chosenCharacter.tutorialChestItems.Count; i++)
            {
                Chest.ChestContent content = new Chest.ChestContent();
                content.contentType = Chest.ChestContent.ChestContentType.Preset;
                content.presetItem = GameManager.session.playerData.chosenCharacter.tutorialChestItems[i];
                chest.contents.Add(content);
            }
        }
    }
}
