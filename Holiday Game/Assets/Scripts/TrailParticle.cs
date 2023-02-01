using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailParticle : MonoBehaviour
{
    public SpriteRenderer sr;
    public bool finished = false;
    public float lifetime = 0.5f;
    public float timer = 0.35f;

    private void Start()
    {
        sr.color = new Color(0.4f, 0.4f, 1.0f, 0.7f);
    }

    private void Update()
    {
        if (!finished)
        {
            float delta = Time.deltaTime;
            timer -= delta;
            if (sr.color.a <= 0)
            {
                finished = true;
                sr.color = new Color(0.4f, 0.4f, 1.0f, 0f);
            }
            else
            {
                sr.color = new Color(0.4f, 0.4f, 1.0f, 0.7f * (timer / lifetime));
            }
        }
    }

    public void SetSprite(Sprite sprite, bool flipX)
    {
        sr.sprite = sprite;
        sr.flipX = flipX;
        timer = lifetime;
        finished = false;
        sr.color = new Color(0.4f, 0.4f, 1.0f, 0.5f);
    }
}
