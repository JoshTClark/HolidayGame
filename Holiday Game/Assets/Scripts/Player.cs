using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Player now derives from StatsComponent. This helps it keep track of all of its stats and helps organize things a lot
public class Player : StatsComponent
{
    private List<Weapon> attacks = new List<Weapon>();

    [SerializeField]
    private InputActionReference movement;

    private void Update()
    {
        float delta = Time.deltaTime;

        // Basic movement
        Vector2 movementInput = movement.action.ReadValue<Vector2>();
        movementInput = movementInput * Speed;
        GetComponent<Rigidbody2D>().velocity = movementInput;
    }

    // Adds an attack to the player
    public void AddAttack(Weapon attack)
    {
        attacks.Add(Instantiate(attack, transform));
    }
}
