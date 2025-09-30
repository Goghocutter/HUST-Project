using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    [SerializeField] private InputActionAsset inputActions;

    public float sensX = 0.12f;   // tune to taste (no deltaTime)
    public float sensY = 0.12f;   // tune to taste
    public bool YRotOnly = false; // keep if you need it

    private InputAction lookAction;
    private float xRotation; // pitch
    private float yRotation; // yaw (stored on player)
    public float playerYRotation; // if other scripts need it

    void Awake()
    {
        lookAction = inputActions.FindActionMap("Player", true).FindAction("Look", true);
    }

    void OnEnable() => lookAction.Enable();
    void OnDisable() => lookAction.Disable();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize yaw from current player rotation (prevents jump on start)
        yRotation = playerObject.transform.eulerAngles.y;
        xRotation = transform.localEulerAngles.x;
        // normalize pitch to -180..180 then clamp
        if (xRotation > 180f) xRotation -= 360f;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    void LateUpdate()
    {
        // Read raw per-frame mouse delta (already frame-based)
        Vector2 look = lookAction.ReadValue<Vector2>();

        float mouseX = look.x * sensX; // NO deltaTime here
        float mouseY = look.y * sensY;

        // accumulate
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerYRotation = yRotation;

        // Apply: yaw on player, pitch on camera
        if (YRotOnly)
        {
            playerObject.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            transform.localRotation = Quaternion.identity; // no pitch when Y-only
        }
        else
        {
            playerObject.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}
