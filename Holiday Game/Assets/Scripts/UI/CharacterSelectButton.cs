using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    public Image characterImage;
    public List<Sprite> characterAnimation;
    public TMP_Text nameObject, descObject;
    public bool isHover = false;
    public string characterName;
    [TextArea(15, 20)]
    public string desc;
    private float scale = 1;
    private int frame = 0;
    private float timer = 0.0f;

    public void Update()
    {
        float delta = Time.deltaTime;

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

        if (isHover)
        {
            timer += delta;
            if (timer >= 0.5f)
            {
                timer = 0f;
                frame++;
                if (frame >= characterAnimation.Count)
                {
                    frame = 0;
                }
                characterImage.sprite = characterAnimation[frame];
            }
        }

        rectTransform.localScale = new Vector3(scale, scale, 1);
    }

    public void OnHoverStart()
    {
        characterImage.gameObject.SetActive(true);
        nameObject.gameObject.SetActive(true);
        descObject.gameObject.SetActive(true);
        characterImage.sprite = characterAnimation[0];
        nameObject.text = characterName;
        descObject.text = desc;
        isHover = true;
    }

    public void OnHoverStop()
    {
        isHover = false;
    }
}
