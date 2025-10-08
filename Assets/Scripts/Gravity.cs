using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Gravity : MonoBehaviour
{
    public float gravity = -20f;
    public float terminalVelocity = -40f;
    public float groundedStick = -2f;

    public float VerticalVelocity { get; private set; }

    CharacterController cc;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool grounded = cc.isGrounded;

        if (grounded && VerticalVelocity < 0f)
        {
            VerticalVelocity = groundedStick;
        }
        else
        {
            VerticalVelocity += gravity * Time.deltaTime;
            if (VerticalVelocity < terminalVelocity) VerticalVelocity = terminalVelocity;
        }
    }

    public void Jump(float speed)
    {
        if (!cc.isGrounded) return; 
        VerticalVelocity = speed;
    }
}
