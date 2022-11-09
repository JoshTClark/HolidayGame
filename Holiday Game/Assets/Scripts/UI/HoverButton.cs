using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverButton : MonoBehaviour
{
    public bool isHover = false;
    private float scale = 1;
    public void Update()
    {
        float delta = Time.unscaledDeltaTime;

        // Hover animation
        RectTransform rectTransform = this.gameObject.GetComponent<RectTransform>();
        if (isHover && scale < 1.25f)
        {
            scale += 5f * delta;
            if (scale > 1.25f)
            {
                scale = 1.25f;
            }
        }
        else if (!isHover && scale > 1f)
        {
            scale -= 5f * delta;
            if (scale < 1f)
            {
                scale = 1f;
            }
        }

        rectTransform.localScale = new Vector3(scale, scale, 1);
    }

    public void OnHoverStart()
    {
        isHover = true;
    }

    public void OnHoverStop()
    {
        isHover = false;
    }
}
