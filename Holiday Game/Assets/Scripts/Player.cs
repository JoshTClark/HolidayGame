using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private float speed = 5;
    private List<Attack> attacks = new List<Attack>();

    [SerializeField]
    private InputActionReference movement;

    private void Update()
    {
        float delta = Time.deltaTime;

        // Basic movement
        Vector2 movementInput = movement.action.ReadValue<Vector2>();
        movementInput = movementInput * speed;
        GetComponent<Rigidbody2D>().velocity = movementInput;
    }

    public void AddAttack(Attack attack)
    {
        attacks.Add(Instantiate(attack, transform));
    }
}
