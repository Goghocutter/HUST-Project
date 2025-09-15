using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Reset downward velocity if grounded
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Call this from PlayerController
    public void Jump(float jumpSpeed)
    {
        if (isGrounded)
            velocity.y = jumpSpeed;
    }
}
