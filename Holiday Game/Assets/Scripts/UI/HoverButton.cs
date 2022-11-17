using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverButton : MonoBehaviour
{
    [SerializeField]
    AudioSource hoverSound;
    public bool isHover = false;
    private float scale = 1;
    public float scaleMax = 1.25f;

    public void Update()
    {
        float delta = Time.unscaledDeltaTime;

        // Hover animation
        RectTransform rectTransform = this.gameObject.GetComponent<RectTransform>();
        if (isHover && scale < scaleMax)
        {
            scale += 5f * delta;
            if (scale > scaleMax)
            {
                scale = scaleMax;
            }
            /*
            if (!hoverSound.isPlaying)
            {
                hoverSound.Play();
            }
            */
        }
        else if (!isHover && scale > 1f)
        {
            scale -= 5f * delta;
            if (scale < 1f)
            {
                scale = 1f;
            }
            //hoverSound.Stop();
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
