using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Tabs : MonoBehaviour
{
    [SerializeField]
    public List<Tab> tabs = new List<Tab>();

    [SerializeField]
    public int selected = 0;

    [SerializeField]
    private CanvasRenderer contentPanel, tabsPanel;

    [SerializeField]
    private Button tabPrefab;

    private List<Button> buttons = new List<Button>();
    private List<CanvasRenderer> panels = new List<CanvasRenderer>();

    private void Update()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (buttons.Count <= i)
            {
                buttons.Add(Instantiate<Button>(tabPrefab, tabsPanel.transform));
            }
        }
        for (int i = 0; i < tabs.Count; i++)
        {
            if (panels.Count <= i)
            {
            }
        }

        while (buttons.Count > tabs.Count)
        {
            buttons.RemoveAt(buttons.Count - 1);
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(i / buttons.Count, 0);
            buttons[i].gameObject.GetComponent<RectTransform>().anchorMax = new Vector2((i + 1) / buttons.Count, 1);
        }
    }

    [System.Serializable]
    public class Tab
    {
        public string title;
        public string content;
    }
}
