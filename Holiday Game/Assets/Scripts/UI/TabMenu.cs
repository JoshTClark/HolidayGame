using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class TabMenu : MonoBehaviour
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

    private void Awake()
    {
    }

    private void Update()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (buttons.Count <= i)
            {
                buttons.Add(Instantiate<Button>(tabPrefab, tabsPanel.transform));
                buttons[i].onClick.AddListener(() =>
                {
                    ButtonSelect(i);
                });
            }
            if (panels.Count <= i)
            {
                GameObject gameObject = new GameObject("Content" + i, typeof(CanvasRenderer), typeof(RectTransform));
                gameObject.GetComponent<RectTransform>().SetParent(contentPanel.transform, false);
                panels.Add(gameObject.GetComponent<CanvasRenderer>());
            }
        }

        while (buttons.Count > tabs.Count)
        {
            if (buttons[buttons.Count - 1])
            {
                DestroyImmediate(buttons[buttons.Count - 1].gameObject);
            }
            buttons.RemoveAt(buttons.Count - 1);
        }

        while (panels.Count > tabs.Count)
        {
            if (panels[panels.Count - 1])
            {
                DestroyImmediate(panels[panels.Count - 1].gameObject);
            }
            panels.RemoveAt(panels.Count - 1);
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.GetComponent<RectTransform>().anchorMin = new Vector2((float)i / buttons.Count, 0);
            buttons[i].gameObject.GetComponent<RectTransform>().anchorMax = new Vector2((i + 1.0f) / buttons.Count, 1);
            buttons[i].GetComponentInChildren<TMP_Text>().text = tabs[i].title;
        }

        if (buttons.Count > selected && buttons[selected])
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (i == selected)
                {
                    buttons[i].gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    panels[i].gameObject.SetActive(true);
                }
                else
                {
                    buttons[i].gameObject.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 1);
                    panels[i].gameObject.SetActive(false);
                }
            }
        }
    }

    [System.Serializable]
    public class Tab
    {
        public string title;
    }

    public void ButtonSelect(int select)
    {
        selected = select;
    }
}
