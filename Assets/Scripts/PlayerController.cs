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

    // --- NEW: Visual + Camera ---
    [Header("Visual (mesh) & Camera")]
    [Tooltip("Assign the visible capsule/character mesh root (child of the player).")]
    [SerializeField] private Transform bodyRoot;

    [Tooltip("Assign your Main Camera transform (can be unparented).")]
    [SerializeField] private Transform cameraTransform;

    [Tooltip("Eye height above feet when standing (meters). Tune to taste.")]
    [SerializeField] private float eyeHeightStand = 1.6f;

    [Tooltip("Eye height above feet when crouched (meters). Tune to taste).")]
    [SerializeField] private float eyeHeightCrouch = 1.0f;

    [Tooltip("How fast camera/mesh interpolate to target pose.")]
    [SerializeField] private float visualLerpSpeed = 12f;

    private Vector2 moveInput;
    private bool isCrouchingHeld;
    private float currentTargetHeight;
    private Vector3 baseCenter; // center when standing

    // NEW: cached mesh + camera baselines
    private Vector3 bodyLocalPosStand;
    private Vector3 bodyLocalScaleStand;
    private Vector3 camOffsetFromFeetXZ; // camera offset XZ relative to player position

    private void Awake()
    {
        if (!characterController) characterController = GetComponent<CharacterController>();
        gravity = GetComponent<Gravity>();

        var map = inputActions.FindActionMap("Player", throwIfNotFound: true);
        moveAction = map.FindAction("Move", throwIfNotFound: true);
        sprintAction = map.FindAction("Sprint", throwIfNotFound: true);
        jumpAction = map.FindAction("Jump", throwIfNotFound: true);
        crouchAction = map.FindAction("Crouch", throwIfNotFound: true);

        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += _ => moveInput = Vector2.zero;

        jumpAction.performed += _ => { if (gravity != null) gravity.Jump(jumpSpeed); };

        // Hold-to-crouch
        crouchAction.performed += _ => isCrouchingHeld = true;
        crouchAction.canceled += _ => isCrouchingHeld = false;

        // Init capsule
        characterController.height = standHeight;
        baseCenter = characterController.center;
        currentTargetHeight = standHeight;
        ApplyCenterFromHeight(standHeight);

        // --- NEW: cache visual/camera baselines ---
        if (bodyRoot != null)
        {
            bodyLocalPosStand = bodyRoot.localPosition;
            bodyLocalScaleStand = bodyRoot.localScale;
        }

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform != null)
        {
            // Store XZ offset so we only drive Y by eye height
            Vector3 localFromFeet = cameraTransform.position - transform.position;
            camOffsetFromFeetXZ = new Vector3(localFromFeet.x, 0f, localFromFeet.z);
        }
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
            sprinting ? walkSpeed * sprintMultiplier :
                        walkSpeed;

        characterController.Move(wishDir * speed * Time.deltaTime);

        // --- Crouch / Stand (smooth) ---
        if (isCrouchingHeld)
            currentTargetHeight = crouchHeight;
        else
            currentTargetHeight = HasHeadroomToStand() ? standHeight : crouchHeight;

        float newHeight = Mathf.MoveTowards(characterController.height, currentTargetHeight, heightChangeSpeed * Time.deltaTime);
        if (!Mathf.Approximately(newHeight, characterController.height))
        {
            characterController.height = newHeight;
            ApplyCenterFromHeight(newHeight);
        }

        // --- NEW: Drive camera + visual mesh to match controller height ---
        float t = Mathf.InverseLerp(standHeight, crouchHeight, characterController.height); // 0=stand,1=crouch

        // Camera height (world Y = feet + eyeHeight)
        if (cameraTransform != null)
        {
            float targetEyeY = Mathf.Lerp(eyeHeightStand, eyeHeightCrouch, t);
            Vector3 targetCamPos = transform.position + camOffsetFromFeetXZ + Vector3.up * targetEyeY;
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetCamPos, visualLerpSpeed * Time.deltaTime);
        }

        // Visible capsule mesh (keep feet planted, scale Y to match controller)
        if (bodyRoot != null)
        {
            // Scale Y to match controller ratio
            float yScale = Mathf.Clamp(characterController.height / standHeight, 0.01f, 10f);
            Vector3 targetScale = new Vector3(bodyLocalScaleStand.x, bodyLocalScaleStand.y * yScale, bodyLocalScaleStand.z);
            bodyRoot.localScale = Vector3.Lerp(bodyRoot.localScale, targetScale, visualLerpSpeed * Time.deltaTime);

            // Move mesh down by half the height loss so feet stay put
            float deltaH = (standHeight - characterController.height);
            Vector3 targetLocalPos = bodyLocalPosStand + Vector3.down * (deltaH * 0.5f);
            bodyRoot.localPosition = Vector3.Lerp(bodyRoot.localPosition, targetLocalPos, visualLerpSpeed * Time.deltaTime);
        }
    }

    // Keep feet on the ground while resizing capsule
    private void ApplyCenterFromHeight(float height)
    {
        // Assumes the player pivot is at the feet.
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
        bool hit = Physics.SphereCast(origin, radius, Vector3.up, out _, extra, ceilingMask, QueryTriggerInteraction.Ignore);
        return !hit;
    }
}
