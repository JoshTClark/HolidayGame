using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    private float timer = 0f;
    public float interval = 1;
    public float maxScale = 1.2f;
    private bool flag;

    private void Update()
    {
        timer += Time.deltaTime;
        float newScale = 1f;
        if (flag)
        {
            float percentage = timer / interval;
            newScale = (maxScale - 1f) * percentage + 1f;
            if (timer >= interval)
            {
                flag = false;
                timer = 0f;
            }
        }
        else
        {
            float percentage = timer / interval;
            newScale = maxScale - (maxScale - 1f) * percentage;
            if (timer >= interval)
            {
                flag = true;
                timer = 0f;
            }
        }

        newScale = Mathf.Clamp(newScale, 1f, maxScale);
        GetComponent<RectTransform>().localScale = new Vector3(newScale, newScale, 1f);
    }
}
