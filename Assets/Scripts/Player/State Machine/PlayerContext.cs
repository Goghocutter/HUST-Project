using UnityEngine;

[DefaultExecutionOrder(-50)] 
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Gravity))]
[RequireComponent(typeof(CharacterController))]
public class PlayerContext : MonoBehaviour
{
    [HideInInspector] public PlayerController motor;
    [HideInInspector] public Gravity gravity;
    [HideInInspector] public CharacterController cc;

    [HideInInspector] public Vector2 Move;
    [HideInInspector] public bool SprintHeld;
    [HideInInspector] public bool CrouchHeld;
    [HideInInspector] public bool JumpPressed;      
    [HideInInspector] public bool InteractPressed;  

    [HideInInspector] public float lastGroundedAt;

    public bool IsGrounded => cc != null && cc.isGrounded;

    void Awake() { Cache(); }

    public void Cache()
    {
        if (!motor) motor = GetComponent<PlayerController>();
        if (!gravity) gravity = GetComponent<Gravity>();
        if (!cc) cc = GetComponent<CharacterController>();
    }

    public bool IsCoyoteGrounded(float window = 0.2f)
        => (Time.time - lastGroundedAt) <= window;

    public bool IsGroundedProbing(float extra = 0.12f, int mask = Physics.DefaultRaycastLayers)
    {
        if (cc == null) return false;
        float radius = Mathf.Max(0.01f, cc.radius * 0.95f);
        Vector3 feet = transform.position + cc.center + Vector3.down * (cc.height * 0.5f - radius + 0.01f);
        return Physics.SphereCast(feet, radius, Vector3.down, out _, extra, mask, QueryTriggerInteraction.Ignore);
    }

    public void ClearPulses() { JumpPressed = false; InteractPressed = false; }
}
