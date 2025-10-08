using UnityEngine;

public class DevHUD : MonoBehaviour
{
    public PlayerStateMachine fsm;
    public PlayerContext ctx;
    public PlayerController motor;
    public Gravity gravity;

    Rect r = new Rect(10, 10, 520, 20);

    void Awake()
    {
        if (!fsm) fsm = GetComponent<PlayerStateMachine>();
        if (!ctx) ctx = GetComponent<PlayerContext>();
        if (!motor) motor = GetComponent<PlayerController>();
        if (!gravity) gravity = GetComponent<Gravity>();
    }

    void OnGUI()
    {
        if (!fsm || !ctx || !motor || !gravity) return;

        var row = new Rect(r);
        GUI.Label(row, $"STATE: {fsm.CurrentStateName}"); row.y += 18;
        GUI.Label(row, $"Move:{ctx.Move}  SprintHeld:{ctx.SprintHeld}  CrouchHeld:{ctx.CrouchHeld}  JumpPressed:{ctx.JumpPressed}"); row.y += 18;
        GUI.Label(row, $"wantSprint:{motor.wantSprint} wantCrouch:{motor.wantCrouch} wantJumpPulse:{motor.wantJumpPulse}"); row.y += 18;
        GUI.Label(row, $"IsGrounded:{ctx.IsGrounded} (probe:{ctx.IsGroundedProbing(0.12f)})  lastGroundedAt:{ctx.lastGroundedAt:F2}  now:{Time.time:F2}"); row.y += 18;
        GUI.Label(row, $"VerticalVel:{gravity.VerticalVelocity:F2}"); row.y += 18;
    }
}
