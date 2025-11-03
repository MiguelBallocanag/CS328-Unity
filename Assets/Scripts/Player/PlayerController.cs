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

    [Header("Jump (Data)")]
    public JumpData jumpData;

    public float Gravity { get; private set; }
    public float JumpVelocity { get; private set; }


    [Header("Jump Input")]
    public bool allowUpToJump = true;
    [Range(0.2f, 0.95f)] public float upThreshold = 0.6f;   // how far stick must push up
    [Range(0.0f, 0.9f)] public float diagMinX = 0.35f;      // min |x| to count as diagonal
    public float dirJumpHorizBoost = 1.5f;                  // small horizontal nudge on directional jump
    public bool requireDiagonalForUpJump = true;            // NEW toggle
    bool _wasGrounded;

    [Header("Dash")]
    public DashData dashData;
    [HideInInspector] public bool dashPressed;

    [Header("Air Dash Limits")]
    public int maxAirDashes = 2;
    [HideInInspector] public int airDashesUsed = 0;

    public float nextDashTime;

    // helper
    public bool CanDashNow =>
    Time.time >= nextDashTime &&
    (IsGrounded || (dashData.allowInAir && airDashesUsed < maxAirDashes));

    public void StartDashCooldown() => nextDashTime = Time.time + dashData.cooldown;

    public void ResetAirDashes() => airDashesUsed = 0;

    public void ConsumeAirDashIfNeeded()
    {
        if (!IsGrounded) airDashesUsed = Mathf.Min(airDashesUsed + 1, maxAirDashes);
    }

    // Input System callback
    public void OnDash(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (ctx.started) dashPressed = true;
    }

    [SerializeField] private Animator animator;       // assign from child "Visuals"
    [SerializeField] private SpriteRenderer sr;       // assign from child "Visuals"
    private Anim animFx;                              // our Anim wrapper
    public void Anim_Jump() => animFx?.Jump();
    public void Anim_Dash() => animFx?.Dash();
    public void Anim_Attack() => animFx?.Attack();



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


    public bool IsGrounded { get; private set; }
    public float lastOnGroundTime;
    public float lastPressedJumpTime;
    public int airJumpsLeft;

    IPlayerState _current;

    [Header("Combat")]
    public AttackData lightAttack;
    [HideInInspector] public bool attackPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>(true);
        jumpData.Compute();
        Gravity = jumpData.gravity;
        JumpVelocity = jumpData.jumpVelocity;

        // auto-find animator/spriteRenderer on Visuals child if not dragged in Inspector
        if (animator == null) animator = GetComponentInChildren<Animator>(true);
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>(true);
        animFx = new Anim(animator);

    }

    void Start()
    {
        SwitchState(new IdleState());
    }

    void Update()
    {
        // --- environment checks
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
        if (IsGrounded) lastOnGroundTime = jumpData.coyoteTime;
        else lastOnGroundTime -= Time.deltaTime;

        lastPressedJumpTime -= Time.deltaTime;

        // --- Detect landing and reset air jumps ---
        if (!_wasGrounded && IsGrounded)
        {
            airJumpsLeft = 1; // or jumpData.maxAirJumps if you use JumpData
            ResetAirDashes();
        }
        _wasGrounded = IsGrounded;

        // --- stick filtering for Up-to-Jump
        Vector2 filtered = ApplyRadialDeadzone(moveInput, stickDeadzone);

        if (allowUpToJump)
        {
            // track how long we've been above/below the relevant bands
            bool aboveUpBand = (filtered.y >= upThreshold) && (filtered.magnitude >= minUpMagnitude);
            bool diagonalOK = !requireDiagonalForUpJump || (Mathf.Abs(filtered.x) >= diagMinX);
            bool belowRearm = (filtered.y < rearmThreshold);

            if (aboveUpBand && diagonalOK && _canDirJump)
                _upHeldTime += Time.deltaTime;
            else
                _upHeldTime = 0f;

            if (belowRearm)
                _rearmBelowTime += Time.deltaTime;
            else
                _rearmBelowTime = 0f;

            // Keyboard: W or W + A/D
            bool kbUp = Keyboard.current?.wKey.wasPressedThisFrame ?? false;
            bool kbDiagRight = kbUp && (Keyboard.current?.dKey.isPressed ?? false);
            bool kbDiagLeft = kbUp && (Keyboard.current?.aKey.isPressed ?? false);

            // Trigger if: keyboard pressed OR stick held up long enough
            bool dirJumpPressed = kbUp || kbDiagRight || kbDiagLeft || (_upHeldTime >= upHoldMinTime);

            // ONLY queue a jump if we’re actually allowed to jump right now
            if (dirJumpPressed && _canDirJump && CanQueueJumpNow())
            {
                lastPressedJumpTime = jumpData.jumpBuffer;
                jumpPressed = true;
                _canDirJump = false; // disarm until we drop below rearm band on the ground
            }

            // Re-arm only when clearly below the rearm band AND we're grounded
            if (IsGrounded && _rearmBelowTime >= rearmMinTime)
                _canDirJump = true;
        }

        if (Mathf.Abs(rb.linearVelocity.x) > 0.01f)
        {
            var s = transform.localScale;
            s.x = Mathf.Sign(rb.linearVelocity.x) * Mathf.Abs(s.x);
            transform.localScale = s;
        }

        // Drive animator params each frame
        if (animFx != null)
        {
            animFx.SetGrounded(IsGrounded);
            animFx.SetSpeed(Mathf.Abs(rb.linearVelocity.x));
            animFx.SetYVel(rb.linearVelocity.y);
        }

        // Flip sprite visually
        if (sr != null && Mathf.Abs(moveInput.x) > 0.05f)
        {
            sr.flipX = moveInput.x < 0f;
        }

        _current?.Tick();

        // clear one-frame flags
        jumpPressed = false;
        dashPressed = false;
        attackPressed = false;
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
            lastPressedJumpTime = jumpData.jumpBuffer;

        }
    }

    public void OnAttack(UnityEngine.InputSystem.InputAction.CallbackContext ctx) {
        if (ctx.started) attackPressed = true;
    }


    // --- helper: radial deadzone for analog stick
    static Vector2 ApplyRadialDeadzone(Vector2 v, float dz)
    {
        float m = v.magnitude;
        if (m <= dz) return Vector2.zero;
        float scaled = Mathf.InverseLerp(dz, 1f, m);
        return v.normalized * scaled;
    }

    // --- helper: consume jump buffer only if ground grace is active
    public bool ConsumeJumpBufferIfAvailable()
    {
        if (lastPressedJumpTime > 0f && lastOnGroundTime > 0f)
        {
            lastPressedJumpTime = 0f;   // consume the buffered press
            lastOnGroundTime = 0f;      // consume coyote window
            return true;
        }
        return false;
    }
    // If you have wall logic, add:  || onWall
    public bool CanQueueJumpNow()
    {
        return IsGrounded || lastOnGroundTime > 0f || airJumpsLeft > 0;
    }
    
    public bool FacingRight {
        get
        {
            if (sr) return !sr.flipX;                      // use sprite facing if available
            if (Mathf.Abs(moveInput.x) > 0.01f) return moveInput.x > 0f; // fallback: infer from input, then velocity
            return rb.linearVelocity.x >= 0f;
        }
    }



}


