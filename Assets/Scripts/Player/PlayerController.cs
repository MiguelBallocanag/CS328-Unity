using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Input flags (one frame)")]
    [HideInInspector] public bool jumpPressed;
    public Vector2 moveInput;

    [Header("Run")]
    public float runSpeed = 8f, groundAccel = 40f, groundDecel = 60f;

    [Header("Jump")]
    public float jumpHeight = 4f, timeToApex = .28f, coyoteTime = .1f, jumpBuffer = .1f;

    [Header("Jump Input")]
    public bool allowUpToJump = true;
    [Range(0.2f, 0.95f)] public float upThreshold = 0.6f;   // how far stick must push up
    [Range(0.0f, 0.9f)]  public float diagMinX = 0.35f;      // min |x| to count as diagonal
    public float dirJumpHorizBoost = 1.5f;                  // small horizontal nudge on directional jump
    public bool requireDiagonalForUpJump = true;            // NEW toggle

    [Header("Up-to-Jump Dwell / Hysteresis")]
    [Range(0.0f, 0.3f)] public float upHoldMinTime = 0.06f; // must hold up >= this to trigger
    [Range(0.0f, 0.3f)] public float rearmMinTime = 0.04f;  // must stay below rearm band for this long

    // timers
    float _upHeldTime = 0f;
    float _rearmBelowTime = 0f;


    [Header("Stick Filtering (Up-to-Jump)")]
    [Range(0f, 0.6f)] public float stickDeadzone = 0.25f;   // ignore tiny stick noise
    [Range(0.5f, 1f)] public float minUpMagnitude = 0.70f;  // overall stick strength needed
    [Range(0.1f, 0.9f)] public float rearmThreshold = 0.30f;// y must fall below this to re-arm

    Vector2 _prevMoveInput;
    Vector2 _prevFiltered;
    bool _canDirJump = true;

    [Header("Air Run")]
    public float airAccel = 25f;      // push while airborne
    public float airDecel = 12f;      // ease toward 0 when no input
    public float airTurnAccel = 38f;  // extra accel when reversing mid-air
    public float maxAirSpeed = 10f;   // optional cap (can use runSpeed instead)


    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;
    public LayerMask groundMask;

    public Rigidbody2D rb { get; private set; }

    public float Gravity { get; private set; }
    public float JumpVelocity { get; private set; }

    public bool IsGrounded { get; private set; }
    public float lastOnGroundTime;
    public float lastPressedJumpTime;
    public int airJumpsLeft;

    IPlayerState _current;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Gravity = (2f * jumpHeight) / (timeToApex * timeToApex);
        JumpVelocity = Gravity * timeToApex;
    }

    void Start()
    {
        SwitchState(new IdleState());
    }

    void Update()
    {
        // --- environment checks
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
        if (IsGrounded) lastOnGroundTime = coyoteTime;
        else lastOnGroundTime -= Time.deltaTime;

        lastPressedJumpTime -= Time.deltaTime;

        // --- stick filtering for Up-to-Jump
        Vector2 filtered = ApplyRadialDeadzone(moveInput, stickDeadzone);

        if (allowUpToJump)
        {
            // track how long we've been above/below the relevant bands
            bool aboveUpBand = (filtered.y >= upThreshold) && (filtered.magnitude >= minUpMagnitude);
            bool diagonalOK  = !requireDiagonalForUpJump || (Mathf.Abs(filtered.x) >= diagMinX);
            bool belowRearm  = (filtered.y < rearmThreshold);

            if (aboveUpBand && diagonalOK && _canDirJump)
                _upHeldTime += Time.deltaTime;
            else
                _upHeldTime  = 0f;

            if (belowRearm)
                _rearmBelowTime += Time.deltaTime;
            else
                _rearmBelowTime  = 0f;

            // Keyboard: W or W + A/D
            bool kbUp        = Keyboard.current?.wKey.wasPressedThisFrame ?? false;
            bool kbDiagRight = kbUp && (Keyboard.current?.dKey.isPressed ?? false);
            bool kbDiagLeft  = kbUp && (Keyboard.current?.aKey.isPressed ?? false);

            // Trigger if: keyboard pressed OR stick held up long enough
            bool dirJumpPressed = kbUp || kbDiagRight || kbDiagLeft || (_upHeldTime >= upHoldMinTime);

            if (dirJumpPressed && _canDirJump)
            {
                lastPressedJumpTime = jumpBuffer;
                jumpPressed = true;
                _canDirJump = false; // disarm until dropped below rearm band long enough
            }

            // Re-arm only after being clearly below rearm band for some time
            if (_rearmBelowTime >= rearmMinTime)
                _canDirJump = true;
        }

        _current?.Tick();

        // clear one-frame flags
        jumpPressed = false;
        _prevMoveInput = moveInput;
        _prevFiltered = filtered;
    }

    public void SwitchState(IPlayerState next)
    {
        _current?.Exit();
        _current = next;
        _current.Enter(this);
    }

    public void OnMove(InputAction.CallbackContext c)
    {
        moveInput = c.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            jumpPressed = true;
            lastPressedJumpTime = jumpBuffer;
        }
    }

    // --- helper: radial deadzone for analog stick
    static Vector2 ApplyRadialDeadzone(Vector2 v, float dz)
    {
        float m = v.magnitude;
        if (m <= dz) return Vector2.zero;
        float scaled = Mathf.InverseLerp(dz, 1f, m);
        return v.normalized * scaled;
    }
}
