using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// INVOKER of the Command pattern.
// Receives raw input (Send Messages), wraps each request in a command
// object, executes it, and stores it in a history list.
// Must be on the SAME GameObject as the PlayerInput component.
public class PlayerInputInvoker : MonoBehaviour
{
    public bool logCommands = true;   // set to false to silence the Console

    private PlayerShooting shooting;  // receiver
    private PlayerMovement movement;  // receiver

    private readonly List<ICommand> history = new List<ICommand>();
    private const int MaxHistory = 50;

    private void Awake()
    {
        shooting = GetComponent<PlayerShooting>();
        movement = GetComponent<PlayerMovement>();
    }

    // ---- Input callbacks (called by PlayerInput, Send Messages) ----
    private void OnShoot() { Run(new StartShootCommand(shooting)); }
    private void OnShootRelease() { Run(new StopShootCommand(shooting)); }
    private void OnReload() { Run(new ReloadCommand(shooting)); }
    private void OnJump() { Run(new JumpCommand(movement)); }

    private void OnMovement(InputValue value)
    {
        Run(new MoveCommand(movement, value.Get<Vector2>()));
    }

    // Execute a command and remember it
    private void Run(ICommand command)
    {
        command.Execute();

        history.Add(command);
        if (history.Count > MaxHistory)
            history.RemoveAt(0);

        if (logCommands)
            Debug.Log("Command executed: " + command.GetType().Name);
    }
}