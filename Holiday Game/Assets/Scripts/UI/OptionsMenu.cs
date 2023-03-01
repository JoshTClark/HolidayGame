using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    private List<Button> tabs = new List<Button>();

    [SerializeField]
    private List<CanvasRenderer> menus = new List<CanvasRenderer>();

    private void Start()
    {
        SelectTab(0);
    }

    /// <summary>
    /// Selects and displays the tab and its options
    /// </summary>
    /// <param name="index"></param>
    public void SelectTab(int index)
    {
        foreach (Button b in tabs)
        {
            b.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            b.gameObject.GetComponentInChildren<TMP_Text>().color = new Color(1f, 1f, 1f, 1f);
        }
        foreach (CanvasRenderer c in menus)
        {
            c.gameObject.SetActive(false);
        }
        tabs[index].gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        tabs[index].GetComponentInChildren<TMP_Text>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
        menus[index].gameObject.SetActive(true);
    }
}
