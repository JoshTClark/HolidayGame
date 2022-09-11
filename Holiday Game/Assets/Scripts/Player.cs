using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private float speed = 5;

    [SerializeField]
    private InputActionReference movement;

    void Update()
    {
        float delta = Time.deltaTime;
        
        // Basic movement
        Vector2 movementInput = movement.action.ReadValue<Vector2>();
        movementInput = movementInput * speed * delta;
        transform.position = transform.position + new Vector3(movementInput.x, movementInput.y, 0);
    }
}
