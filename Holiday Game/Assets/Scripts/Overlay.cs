using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlay : MonoBehaviour
{
    public ResourceManager.OverlayIndex index;
    public SpriteRenderer spriteRenderer;

    private void Update()
    {
        if (spriteRenderer && !this.gameObject.GetComponent<SpriteRenderer>().sprite) { 
            this.gameObject.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
        }
    }
}
