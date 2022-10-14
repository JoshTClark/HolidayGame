using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
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
        RectTransform rect = canvas.GetComponent<RectTransform>();
        currentPosition = spawnPosition;
        Vector2 viewPosition = cam.WorldToViewportPoint(spawnPosition);
        Vector2 canvasPosition = new Vector2(((viewPosition.x * rect.sizeDelta.x) * canvas.scaleFactor), ((viewPosition.y * rect.sizeDelta.y) * canvas.scaleFactor));
        this.gameObject.GetComponent<RectTransform>().position = canvasPosition;
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        RectTransform objectRect = this.gameObject.GetComponent<RectTransform>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Moving the number
        currentPosition += speed * direction * delta;
        Vector2 viewPosition = cam.WorldToViewportPoint(currentPosition);
        Vector2 canvasPosition = new Vector2(((viewPosition.x * canvasRect.sizeDelta.x) * canvas.scaleFactor), ((viewPosition.y * canvasRect.sizeDelta.y) * canvas.scaleFactor));
        objectRect.position = canvasPosition;

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
