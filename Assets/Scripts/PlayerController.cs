using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Reference character controller
    [SerializeField] private CharacterController characterController;

    // Reference input actions asset
    [SerializeField] private InputActionAsset inputActions;

    // Input variables
    private InputAction moveAction;
    private Vector2 moveInput;
    private InputAction sprintAction;
    private InputAction jumpAction;

    private Gravity gravity;

    // Move variables
    private float moveSpeed = 6f;
    private float sprintMultiplier = 2f;
    private Vector3 velocity;
    //private float gravity = -9.81f;
    [SerializeField] private float jumpSpeed = 5.5f; // passed to Gravity.Jump()


    private void Awake()
    {
        gravity = GetComponent<Gravity>();

        // Use Awake(), not Start() method for these

        // Gets the input action asset and actions
        moveAction = inputActions.FindActionMap("Player").FindAction("Move");
        sprintAction = inputActions.FindActionMap("Player").FindAction("Sprint");
        jumpAction = inputActions.FindActionMap("Player").FindAction("Jump");

        // Reads moveAction value
        moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
        // Cancels the action when not moving
        moveAction.canceled += context => moveInput = Vector2.zero;
        jumpAction.performed += _ =>
        {
            gravity.Jump(jumpSpeed);
        };
    }

    // Allways use OnEnable and OnDisable functions when working with new input system
    private void OnEnable()
    {
        moveAction.Enable();
        sprintAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        sprintAction.Disable();
        jumpAction.Disable();
    }

    private void FixedUpdate()
    {
        // Movement
        Vector2 move = moveInput;

        float speedMultiplier = sprintAction.ReadValue<float>() > 0 ? sprintMultiplier : 1f;

        Vector3 movement = (move.y * transform.forward) + (move.x * transform.right);
        characterController.Move(movement * moveSpeed * speedMultiplier * Time.deltaTime);

        characterController.Move(velocity * Time.deltaTime);

        // - Run when left shift is held
        if (sprintAction.triggered)
        {
            moveSpeed = 5f;
        }
        // - Return to walk speed when left shift is released
        if (!sprintAction.triggered)
        {
            moveSpeed = 3f;
        }
    }
}
