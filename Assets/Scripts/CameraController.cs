using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    // Reference player object
    [SerializeField] private GameObject playerObject;

    // Reference input actions asset
    [SerializeField] private InputActionAsset inputActions;

    // Input variables
    private InputAction lookAction;
    private Vector2 lookInput;

    // Toggle xRotation
    public bool YRotOnly;

    // Mouse sensitivity. Can be changed in inspector
    public float sensX = 50;
    public float sensY = 50;

    // Used to get the mouse's X and Y rotation
    private float xRotation;
    private float yRotation;

    public float playerYRotation; // Used for rotating sprite objects and enemies

    private void Awake()
    {
        // Use Awake(), not Start() method for these

        // Gets the input action asset and action
        lookAction = inputActions.FindActionMap("Player").FindAction("Look");

        // Reads lookAction value
        lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
        // Cancels the action when not moving mouse
        lookAction.canceled += context => lookInput = Vector2.zero;
    }

    private void Start()
    {
        // Locks cursor
        Cursor.lockState = CursorLockMode.Locked;
        // Hides cursor
        Cursor.visible = false;
    }

    // Allways use OnEnable and OnDisable functions when working with new input system
    private void OnEnable()
    {
        lookAction.Enable();
    }

    private void OnDisable()
    {
        lookAction.Disable();
    }

    private void Update()
    {
        // Get mouse input
        float mouseX = lookInput.x * sensX * Time.deltaTime;
        float mouseY = lookInput.y * sensY * Time.deltaTime;

        // Plug mouse input into xRotation and yRotation with 90 degree clamp on xRotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        yRotation += mouseX;


        playerYRotation = yRotation; // Used for rotating sprite objects and enemies in SpriteFacePlayer script


        // Rotates the camera and player object
        if (YRotOnly)
        {
            transform.rotation = Quaternion.Euler(0, yRotation, 0);
            playerObject.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            playerObject.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}
