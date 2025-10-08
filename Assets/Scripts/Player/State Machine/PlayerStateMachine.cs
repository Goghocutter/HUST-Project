using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerContext))]
public class PlayerStateMachine : MonoBehaviour
{
    public enum Locomotion { Idle, WalkRun, Jump, Crouch, LedgeGrab, Interact, Dead }

    private PlayerContext ctx;
    private IPlayerState state;

    [SerializeField] private InputActionAsset input; // Action Map name: "Player"
    private InputAction move, sprint, crouch, jump, interact;

    public string CurrentStateName => state?.GetType().Name ?? "NULL";
    public event System.Action<string, string> OnStateChanged;

    void Awake()
    {
        ctx = GetComponent<PlayerContext>();
        if (input == null)
        {
            Debug.LogError("[FSM] InputActionAsset not assigned.");
            enabled = false; return;
        }

        var map = input.FindActionMap("Player", true);
        move = map.FindAction("Move", true);
        sprint = map.FindAction("Sprint", true);
        crouch = map.FindAction("Crouch", true);
        jump = map.FindAction("Jump", true);
        interact = map.FindAction("Interact", throwIfNotFound: false);
    }

    void OnEnable() { move.Enable(); sprint.Enable(); crouch.Enable(); jump.Enable(); interact?.Enable(); }
    void OnDisable() { move.Disable(); sprint.Disable(); crouch.Disable(); jump.Disable(); interact?.Disable(); }

    void Start()
    {
        ctx.Cache();
        state = new IdleState();
        state.Enter(ctx);
    }

    void Update()
    {
        ctx.Move = move.ReadValue<Vector2>();
        ctx.SprintHeld = sprint.ReadValue<float>() > 0.5f;
        ctx.CrouchHeld = crouch.ReadValue<float>() > 0.5f;
        if (jump.WasPressedThisFrame()) ctx.JumpPressed = true;
        if (interact != null && interact.WasPressedThisFrame()) ctx.InteractPressed = true;

        if (ctx.IsGrounded) ctx.lastGroundedAt = Time.time;

        ctx.motor.moveInput = ctx.Move;

        state.Tick(ctx, Time.deltaTime);

        var next = state.TryTransition(ctx);
        if (next != null)
        {
            var old = CurrentStateName;
            state.Exit(ctx);
            state = next;
            state.Enter(ctx);
            OnStateChanged?.Invoke(old, CurrentStateName);
        }

        ctx.ClearPulses();
    }
}
