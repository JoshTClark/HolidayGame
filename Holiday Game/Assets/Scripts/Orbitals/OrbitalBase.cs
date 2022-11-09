using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalBase : MonoBehaviour
{
    public ResourceManager.OrbitalIndex index;
    public float speed = 1f;
    public float distance = 5f;
    public GameObject child;

    private void Update()
    {
        float delta = Time.deltaTime;
        child.transform.localPosition = new Vector2(distance, 0);

        transform.Rotate(new Vector3(0, 0, delta * speed * Mathf.Rad2Deg));
    }
}
