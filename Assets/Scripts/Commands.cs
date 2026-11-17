using UnityEngine;

// ============================================================
//  COMMAND PATTERN (GoF, Behavioral)
//  Command interface : ICommand
//  Concrete commands : StartShootCommand, StopShootCommand,
//                      ReloadCommand, JumpCommand, MoveCommand
//  Receivers         : PlayerShooting, PlayerMovement
//  Invoker           : PlayerInputInvoker
//  Client            : the Input System (creates the request)
// ============================================================

// Command interface: every request becomes an object with Execute()
public interface ICommand
{
    void Execute();
}

// ---------------- SHOOT (hold) ----------------
public class StartShootCommand : ICommand
{
    private readonly PlayerShooting receiver;
    public StartShootCommand(PlayerShooting receiver) { this.receiver = receiver; }
    public void Execute() { receiver.StartShooting(); }
}

// ---------------- SHOOT (release) ----------------
public class StopShootCommand : ICommand
{
    private readonly PlayerShooting receiver;
    public StopShootCommand(PlayerShooting receiver) { this.receiver = receiver; }
    public void Execute() { receiver.StopShooting(); }
}

// ---------------- RELOAD ----------------
public class ReloadCommand : ICommand
{
    private readonly PlayerShooting receiver;
    public ReloadCommand(PlayerShooting receiver) { this.receiver = receiver; }
    public void Execute() { receiver.Reload(); }
}

// ---------------- JUMP ----------------
public class JumpCommand : ICommand
{
    private readonly PlayerMovement receiver;
    public JumpCommand(PlayerMovement receiver) { this.receiver = receiver; }
    public void Execute() { receiver.Jump(); }
}

// ---------------- MOVE (command with a parameter) ----------------
public class MoveCommand : ICommand
{
    private readonly PlayerMovement receiver;
    private readonly Vector2 direction;

    public MoveCommand(PlayerMovement receiver, Vector2 direction)
    {
        this.receiver = receiver;
        this.direction = direction;
    }

    public void Execute() { receiver.SetMoveInput(direction); }
}