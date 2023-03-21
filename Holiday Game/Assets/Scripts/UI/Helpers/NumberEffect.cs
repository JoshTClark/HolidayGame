using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberEffect : MonoBehaviour
{
    public Canvas canvas;
    public Camera cam;
    public Vector2 spawnPosition;
    private Vector2 direction;
    private Vector2 currentPosition;
    public float speed = 1f;
    private float timer = 0f;
    public float fadeTime = 1f;
    public float size = 1f;
    public Color color = new Color(1f, 1f, 1f, 1f);

    private void Start()
    {
        // Direction for the number to float
        direction = new Vector2(Random.Range(-0.2f, 0.2f), 1).normalized;

        // Converting from world space to canvas/screen space
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        currentPosition += spawnPosition;
        Vector2 viewPosition = cam.WorldToViewportPoint(currentPosition);
        Vector2 canvasPosition = new Vector2(((viewPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)), ((viewPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        this.gameObject.GetComponent<RectTransform>().anchoredPosition = canvasPosition;
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        // Moving the effect
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        currentPosition += speed * direction * delta;
        Vector2 viewPosition = cam.WorldToViewportPoint(currentPosition);
        Vector2 canvasPosition = new Vector2(((viewPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)), ((viewPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        this.gameObject.GetComponent<RectTransform>().anchoredPosition = canvasPosition;

        timer += delta;
        if (timer <= fadeTime)
        {
            TMP_Text text = this.gameObject.GetComponent<TMP_Text>();
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(1.0f, 0f, timer / fadeTime));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
