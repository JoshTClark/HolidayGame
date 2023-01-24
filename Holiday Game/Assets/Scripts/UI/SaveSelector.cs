using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSelector : MonoBehaviour
{
    [SerializeField]
    private TMP_Text display;

    public int selectedSlot = 0;
    public List<GameData> slots = new List<GameData>();
    private string displayText = "";


    private void Update()
    {
        if (slots.Count == 0)
        {
            displayText = "No saves found";
        }
        else
        {
            displayText = slots[selectedSlot].saveName;
        }

        display.text = displayText;
    }

    public void ButtonRight()
    {
        selectedSlot++;
        if (selectedSlot >= slots.Count)
        {
            selectedSlot = slots.Count - 1;
        }
        if (selectedSlot < 0)
        {
            selectedSlot = 0;
        }
    }

    public void ButtonLeft()
    {
        selectedSlot--;
        if (selectedSlot < 0)
        {
            selectedSlot = 0;
        }
    }

    public void AddSlot(GameData data)
    {
        slots.Add(data);
    }

    public GameData GetSelected()
    {
        if (slots.Count > 0)
        {
            return slots[selectedSlot];
        }
        return null;
    }

    public void DeleteSlot()
    {
        SaveManager.Delete(slots[selectedSlot].saveName);
        slots.RemoveAt(selectedSlot);
        if (slots.Count == 0)
        {
            displayText = "No saves found";
            selectedSlot= 0;
        }
        else
        {
            //displayText = slots[selectedSlot].saveName;
            ButtonLeft();
        }
    }
}
