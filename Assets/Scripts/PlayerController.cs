using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private InputActionAsset inputActions;
    private Gravity gravity; // your separate gravity script

    [Header("Actions (Player map)")]
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintMultiplier = 1.7f;
    [SerializeField] private float crouchMoveSpeed = 3f;

    [Header("Crouch (capsule)")]
    [SerializeField] private float standHeight = 2.0f;
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float heightChangeSpeed = 10f; // how fast the capsule resizes
    [SerializeField] private LayerMask ceilingMask = ~0;     // what blocks standing; default: everything

    [Header("Jump")]
    [SerializeField] private float jumpSpeed = 5.5f; // given to Gravity.Jump()

    private Vector2 moveInput;
    private bool isCrouchingHeld;
    private float currentTargetHeight;
    private Vector3 baseCenter; // center when standing

    private void Awake()
    {
        if (!characterController) characterController = GetComponent<CharacterController>();
        gravity = GetComponent<Gravity>();

        var map = inputActions.FindActionMap("Player", throwIfNotFound: true);
        moveAction   = map.FindAction("Move",   throwIfNotFound: true);
        sprintAction = map.FindAction("Sprint", throwIfNotFound: true);
        jumpAction   = map.FindAction("Jump",   throwIfNotFound: true);
        crouchAction = map.FindAction("Crouch", throwIfNotFound: true);

        moveAction.performed  += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled   += _   => moveInput = Vector2.zero;

        // Jump: trigger on performed
        jumpAction.performed  += _ =>
        {
            if (gravity != null)
                gravity.Jump(jumpSpeed);
        };

        // Crouch: hold-to-crouch
        crouchAction.performed += _ => isCrouchingHeld = true;
        crouchAction.canceled  += _ => isCrouchingHeld = false;

        // Init capsule
        characterController.height = standHeight;
        baseCenter = characterController.center;
        currentTargetHeight = standHeight;
        ApplyCenterFromHeight(standHeight);
    }

    private void OnEnable()
    {
        moveAction.Enable();
        sprintAction.Enable();
        jumpAction.Enable();
        crouchAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        sprintAction.Disable();
        jumpAction.Disable();
        crouchAction.Disable();
    }

    private void Update()
    {
        // --- Horizontal movement ---
        var move = moveInput;
        var wishDir = (move.y * transform.forward) + (move.x * transform.right);
        wishDir = Vector3.ClampMagnitude(wishDir, 1f);

        bool sprinting = sprintAction.ReadValue<float>() > 0.5f;
        float speed =
            isCrouchingHeld ? crouchMoveSpeed :
            sprinting        ? walkSpeed * sprintMultiplier :
                               walkSpeed;

        characterController.Move(wishDir * speed * Time.deltaTime);

        // --- Crouch / Stand (smooth) ---
        if (isCrouchingHeld)
        {
            currentTargetHeight = crouchHeight;
        }
        else
        {
            // Only stand if there's headroom
            if (HasHeadroomToStand())
                currentTargetHeight = standHeight;
            else
                currentTargetHeight = crouchHeight;
        }

        float newHeight = Mathf.MoveTowards(characterController.height, currentTargetHeight, heightChangeSpeed * Time.deltaTime);
        if (!Mathf.Approximately(newHeight, characterController.height))
        {
            characterController.height = newHeight;
            ApplyCenterFromHeight(newHeight);
        }
    }

    // Keep feet on the ground while resizing capsule
    private void ApplyCenterFromHeight(float height)
    {
        // CharacterController center is relative to object pivot.
        // If your pivot is at feet, center.y should be height/2.
        // If not, adjust baseCenter accordingly.
        characterController.center = new Vector3(baseCenter.x, height * 0.5f, baseCenter.z);
    }

    // Prevent popping up into ceilings when trying to stand
    private bool HasHeadroomToStand()
    {
        float radius = characterController.radius * 0.95f;
        float current = characterController.height;
        float extra = (standHeight - current) + 0.05f;

        if (extra <= 0f) return true;

        // Cast upward from current top
        Vector3 origin = transform.position + characterController.center + Vector3.up * (current * 0.5f - radius);
        // Sphere cast up by "extra"; if hit => no headroom
        bool hit = Physics.SphereCast(origin, radius, Vector3.up, out _, extra, ceilingMask, QueryTriggerInteraction.Ignore);
        return !hit;
    }
}
