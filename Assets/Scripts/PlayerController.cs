using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Gravity))]
public class PlayerController : MonoBehaviour
{
    [Header("Speeds")]
    public float walkSpeed = 4.0f;
    public float sprintMultiplier = 1.6f;
    public float crouchMoveSpeed = 2.0f;

    [Header("Capsule / Crouch")]
    public float standHeight = 1.8f;
    public float crouchHeight = 1.1f;
    public float heightChangeSpeed = 6f;
    public Vector3 controllerCenterStand = new Vector3(0, 0.9f, 0);
    public Vector3 controllerCenterCrouch = new Vector3(0, 0.55f, 0);

    [Header("Camera/Visuals (optional)")]
    public Transform cameraTransform;
    public float eyeHeightStand = 1.65f;
    public float eyeHeightCrouch = 1.0f;
    public Vector3 camOffsetFromFeetXZ = Vector3.zero;
    public Transform bodyRoot;                          
    public float visualLerpSpeed = 12f;

    [Header("Jump")]
    public float jumpSpeed = 6.0f;

    [HideInInspector] public bool wantSprint;
    [HideInInspector] public bool wantCrouch;
    [HideInInspector] public bool wantJumpPulse;

    [HideInInspector] public Vector2 moveInput;

    CharacterController cc;
    Gravity gravity;

    float currentTargetHeight;
    Vector3 bodyLocalScaleStand = Vector3.one;
    Vector3 bodyLocalPosStand = Vector3.zero;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        gravity = GetComponent<Gravity>();

        // initialize controller
        cc.height = standHeight;
        cc.center = controllerCenterStand;
        currentTargetHeight = standHeight;

        if (bodyRoot != null)
        {
            bodyLocalScaleStand = bodyRoot.localScale;
            bodyLocalPosStand = bodyRoot.localPosition;
        }
    }

    void Update()
    {
        Vector3 wishDir = Vector3.zero;
        {
            var move = moveInput;
            wishDir = (move.y * transform.forward) + (move.x * transform.right);
            wishDir = Vector3.ClampMagnitude(wishDir, 1f);
        }

        bool crouching = wantCrouch;
        bool sprinting = wantSprint && !crouching;

        float speed = crouching ? crouchMoveSpeed :
                      sprinting ? walkSpeed * sprintMultiplier :
                                  walkSpeed;

        Vector3 horizontal = wishDir * speed;
        Vector3 velocity = new Vector3(horizontal.x, gravity.VerticalVelocity, horizontal.z);

        cc.Move(velocity * Time.deltaTime);

        if (crouching)
        {
            currentTargetHeight = crouchHeight;
            cc.center = Vector3.Lerp(cc.center, controllerCenterCrouch, Time.deltaTime * heightChangeSpeed);
        }
        else
        {
            currentTargetHeight = HasHeadroomToStand() ? standHeight : crouchHeight;
            cc.center = Vector3.Lerp(cc.center,
                (Mathf.Approximately(currentTargetHeight, standHeight) ? controllerCenterStand : controllerCenterCrouch),
                Time.deltaTime * heightChangeSpeed);
        }

        float newHeight = Mathf.MoveTowards(cc.height, currentTargetHeight, heightChangeSpeed * Time.deltaTime);
        if (!Mathf.Approximately(newHeight, cc.height))
            cc.height = newHeight;

        ConsumeJumpPulseIfAny(s => gravity.Jump(s), jumpSpeed);


        float t = Mathf.InverseLerp(standHeight, crouchHeight, cc.height); // 0=stand,1=crouch

        if (cameraTransform != null)
        {
            float targetEyeY = Mathf.Lerp(eyeHeightStand, eyeHeightCrouch, t);
            Vector3 targetCamPos = transform.position + camOffsetFromFeetXZ + Vector3.up * targetEyeY;
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetCamPos, visualLerpSpeed * Time.deltaTime);
        }

        if (bodyRoot != null)
        {
            float yScale = Mathf.Clamp(cc.height / standHeight, 0.01f, 10f);
            Vector3 targetScale = new Vector3(bodyLocalScaleStand.x, bodyLocalScaleStand.y * yScale, bodyLocalScaleStand.z);
            bodyRoot.localScale = Vector3.Lerp(bodyRoot.localScale, targetScale, visualLerpSpeed * Time.deltaTime);

            float deltaH = (standHeight - cc.height);
            Vector3 targetLocalPos = bodyLocalPosStand + Vector3.down * (deltaH * 0.5f);
            bodyRoot.localPosition = Vector3.Lerp(bodyRoot.localPosition, targetLocalPos, visualLerpSpeed * Time.deltaTime);
        }
    }

    public void ConsumeJumpPulseIfAny(System.Action<float> onJump, float jumpSpeedValue)
    {
        if (!wantJumpPulse) return;
        onJump?.Invoke(jumpSpeedValue);
        wantJumpPulse = false;
    }

    bool HasHeadroomToStand()
    {
        float radius = Mathf.Max(0.1f, cc.radius * 0.95f);
        Vector3 head = transform.position + cc.center + Vector3.up * (cc.height * 0.5f - radius);
        return !Physics.SphereCast(head, radius * 0.95f, Vector3.up, out _, 0.15f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
    }
}
