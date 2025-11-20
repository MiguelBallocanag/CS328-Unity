// PlayerController.cs
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Input flags (one frame)")]
    [HideInInspector] public bool jumpPressed;
    public Vector2 moveInput;

    [Header("Run")]
    public float runSpeed = 5.8f, groundAccel = 70f, groundDecel = 100f;

    [Header("Air Run")]
    public float airAccel = 20f;
    public float airDecel = 14f;
    public float airTurnAccel = 32f;
    public float maxAirSpeed = 6.8f;

    [Header("Ground Stop Tuning")]
    public float stopThreshold = 0.05f;
    public float idleBrake = 120f;

    [Header("Jump (Data)")]
    [HideInInspector] public bool jumpHeld;
    public JumpData jumpData;
    public float Gravity { get; private set; }
    public float JumpVelocity { get; private set; }

    [Header("Jump Input")]
    public bool allowUpToJump = true;
    [Range(0.2f, 0.95f)] public float upThreshold = 0.6f;
    [Range(0.0f, 0.9f)] public float diagMinX = 0.35f;
    public float dirJumpHorizBoost = 1.5f;
    public bool requireDiagonalForUpJump = true;

    [Header("Up-to-Jump Dwell / Hysteresis")]
    [Range(0.0f, 0.3f)] public float upHoldMinTime = 0.06f;
    [Range(0.0f, 0.3f)] public float rearmMinTime = 0.04f;

    [Header("Stick Filtering (Up-to-Jump)")]
    [Range(0f, 0.6f)] public float stickDeadzone = 0.25f;
    [Range(0.5f, 1f)] public float minUpMagnitude = 0.70f;
    [Range(0.1f, 0.9f)] public float rearmThreshold = 0.30f;

    [Header("Air Jumps")]
    public bool enableAirJumps = false;

    [Header("Vertical Speed Clamp")]
    public bool clampVerticalSpeed = true;
    public float maxRiseSpeed = 8f;
    public float maxFallSpeed = 14f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;
    public LayerMask groundMask;

    [Header("Combat")]
    public AttackData lightAttack;
    [HideInInspector] public bool attackPressed;
    public float attackBuffer = 0.20f;

    [Header("Dash")]
    public DashData dashData;
    float _dashStreakUntil;
    int _dashStreakCount;

    int _lastAttackAnimFrame = -1; // keep

    [Header("Hazard Effects")]
    public float swampSpeedMult = 1f;
    public bool swampJumpLock = false;
    public float swampJumpMult = 1f;


    // =========================
    // Assists
    // =========================
    [Header("Assists — Platform Snap")]
    public bool enablePlatformSnap = true;
    [Tooltip("Probe distance below groundCheck to snap onto platforms")]
    public float snapProbe = 0.18f;
    [Tooltip("Only snap if vertical speed is not faster than this downward")]
    public float snapVYThreshold = -8f;
    [Tooltip("Max slope (deg) allowed to snap to")]
    public float snapMaxSlope = 40f;

    [Header("Assists — Apex Hang")]
    public bool enableApexHang = true;
    [Tooltip("If |vy| is inside this band near apex, reduce gravity")]
    public float apexVyBand = 0.8f;
    [Tooltip("Gravity multiplier near apex (0.5 = half gravity)")]
    [Range(0.2f, 1.0f)] public float apexHangMult = 0.55f;

    [Header("Assists — Apex Steer")]
    public bool enableApexSteer = true;
    [Tooltip("If |vy| <= this near apex, increase air accel & cap")]
    public float apexSteerVyBand = 1.2f;
    [Tooltip("Multiply air accel when within apex band")]
    public float apexAccelBoost = 1.5f;
    [Tooltip("Multiply max air speed when within apex band")]
    public float apexMaxAirSpeedBoost = 1.10f;

    [Header("Assists — Grace Landing")]
    public bool enableGraceLanding = true;
    [Tooltip("Time after landing to be forgiving (bigger ground circle / extra brake)")]
    public float graceTime = 0.08f;
    [Tooltip("Ground check radius multiplier during grace")]
    public float graceRadiusMult = 1.35f;
    [Tooltip("Extra braking force while not pressing X during grace")]
    public float graceFrictionBrake = 200f;

    // Landing grace timer
    float _landingGraceUntil;
    public bool InLandingGrace => enableGraceLanding && (Time.time < _landingGraceUntil);
    public void StartLandingGrace() { if (enableGraceLanding) _landingGraceUntil = Time.time + graceTime; }

    public void RegisterDashForStreak() {
        float now = Time.time;
        if (now <= _dashStreakUntil) {
            _dashStreakCount++;
        } else {
            _dashStreakCount = 1;
            _dashStreakUntil = now + dashData.streakWindow;
        }
        if (_dashStreakCount >= dashData.streakLimit) {
            nextDashTime = Mathf.Max(nextDashTime, now + dashData.postStreakLockout);
            _dashStreakCount = 0;
            _dashStreakUntil = 0f;
        }
    }

    [HideInInspector] public bool justDashed;
    [HideInInspector] public bool dashPressed;

    [Header("Air Dash Limits")]
    public int maxAirDashes = 2;
    [HideInInspector] public int airDashesUsed = 0;

    public float nextDashTime;

    public bool CanDashNow =>
        Time.time >= nextDashTime &&
        (IsGrounded || (dashData.allowInAir && airDashesUsed < maxAirDashes));

    public void StartDashCooldown() => nextDashTime = Time.time + dashData.cooldown;
    public void ResetAirDashes() => airDashesUsed = 0;

    public void ConsumeAirDashIfNeeded()
    {
        if (!IsGrounded) airDashesUsed = Mathf.Min(airDashesUsed + 1, maxAirDashes);
    }

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sr;
    private Anim animFx;

    public void Anim_Jump()  => animFx?.Jump();
    public void Anim_Dash()  => animFx?.Dash();
    public void Anim_AttackTrigger() => animFx?.AttackTrigger();
    public void Anim_PlayAttackFromStart() => animFx?.PlayAttackFromStart();
    public bool Anim_IsInAttack() => animFx != null && animFx.IsInAttack();

    public bool TryStartAttackAnim() {
        if (Time.frameCount == _lastAttackAnimFrame) return false;
        _lastAttackAnimFrame = Time.frameCount;

        if (!Anim_IsInAttack()) Anim_AttackTrigger();
        else Anim_PlayAttackFromStart();
        return true;
    }

    public Rigidbody2D rb { get; private set; }
    public bool IsGrounded { get; private set; }
    public float lastOnGroundTime;
    public float lastPressedJumpTime;
    public int airJumpsLeft;

    IPlayerState _current;

    bool _wasGrounded;
    float _upHeldTime = 0f;
    float _rearmBelowTime = 0f;
    Vector2 _prevMoveInput;
    Vector2 _prevFiltered;
    bool _canDirJump = true;
    private float _attackBufferUntil;

    public bool AttackBuffered => Time.time <= _attackBufferUntil;

    public void ConsumeAttackBuffer() {
        _attackBufferUntil = 0f;
        attackPressed = false;
    }

    void ApplyJumpTuning()
    {
        if (!jumpData) return;
        jumpData.Compute();
        Gravity = jumpData.gravity;
        // Apply both multipliers: base tuning (in jumpVelocity) + hazard effect (swampJumpMult)
        JumpVelocity = jumpData.jumpVelocity * swampJumpMult;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>(true);
        ApplyJumpTuning();

        if (animator == null) animator = GetComponentInChildren<Animator>(true);
        animFx = new Anim(animator);
    }

    void Start()
    {
        SwitchState(new IdleState());
    }

    // -------- Input & non-physics runs per-rendered-frame --------
    void Update()
    {
        ApplyJumpTuning();

        // Grace landing: inflate ground check radius briefly after landing
        float radius = groundCheckRadius * (InLandingGrace ? graceRadiusMult : 1f);
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, radius, groundMask);
        if (IsGrounded) lastOnGroundTime = jumpData.coyoteTime;
        else lastOnGroundTime -= Time.deltaTime;

        lastPressedJumpTime -= Time.deltaTime;

        // landing transition -> start grace
        if (!_wasGrounded && IsGrounded)
        {
            airJumpsLeft = enableAirJumps ? jumpData.maxAirJumps : 0;
            ResetAirDashes();
            StartLandingGrace();
        }
        _wasGrounded = IsGrounded;

        Vector2 filtered = ApplyRadialDeadzone(moveInput, stickDeadzone);

        if (allowUpToJump)
        {
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

            bool kbUp = Keyboard.current?.wKey.wasPressedThisFrame ?? false;
            bool kbDiagRight = kbUp && (Keyboard.current?.dKey.isPressed ?? false);
            bool kbDiagLeft  = kbUp && (Keyboard.current?.aKey.isPressed ?? false);

            bool dirJumpPressed = kbUp || kbDiagRight || kbDiagLeft || (_upHeldTime >= upHoldMinTime);

            if (dirJumpPressed && _canDirJump && CanQueueJumpNow())
            {
                lastPressedJumpTime = jumpData.jumpBuffer;
                jumpPressed = true;
                _canDirJump = false;
            }

            if (IsGrounded && _rearmBelowTime >= rearmMinTime)
                _canDirJump = true;
        }

        if (animFx != null)
        {
            animFx.SetGrounded(IsGrounded);
            float animSpeed = (Mathf.Abs(moveInput.x) > 0.1f) ? Mathf.Abs(rb.linearVelocity.x) : 0f;
            animFx.SetSpeed(animSpeed);
            animFx.SetYVel(rb.linearVelocity.y);
        }

        if (sr != null && Mathf.Abs(moveInput.x) > 0.05f)
        {
            sr.flipX = moveInput.x < 0f;
        }

        // Treat Space + W + stick-up (when allowed) + gamepad South as "held jump"
        bool kbSpaceHeld = Keyboard.current?.spaceKey.isPressed ?? false;
        bool kbWHeld     = Keyboard.current?.wKey.isPressed ?? false;

        bool stickUpHeld = false;
        if (allowUpToJump)
        {
            bool aboveUpBand = (filtered.y >= upThreshold) && (filtered.magnitude >= minUpMagnitude);
            bool diagonalOK  = !requireDiagonalForUpJump || (Mathf.Abs(filtered.x) >= diagMinX);
            stickUpHeld = aboveUpBand && diagonalOK;
        }

        jumpHeld =
            kbSpaceHeld ||
            kbWHeld     ||
            stickUpHeld ||
            (Gamepad.current?.buttonSouth.isPressed ?? false);
    }

    // -------- Physics & state machine run at fixed timestep --------
    void FixedUpdate()
    {
        _current?.Tick();

        if (clampVerticalSpeed)
        {
            var v = rb.linearVelocity;
            if (v.y < -maxFallSpeed) v.y = -maxFallSpeed;
            // NOTE: Not clamping rise speed to allow full jump height
            rb.linearVelocity = v;
        }

        if (swampSpeedMult < 1f)
        {
            var v = rb.linearVelocity;
            v.x *= swampSpeedMult;
            rb.linearVelocity = v;
        }

        // Clear one-frame input flags AFTER physics consumed them
        jumpPressed = false;
        dashPressed = false;
        attackPressed = false;
    }

    public void OnMove(InputAction.CallbackContext c)
    {
        moveInput = c.ReadValue<Vector2>();
        moveInput.x *= swampSpeedMult;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            jumpPressed = true;
            lastPressedJumpTime = jumpData.jumpBuffer;
        }
        // intentionally do NOT set jumpHeld here; we poll each frame in Update()
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.started && CanDashNow) dashPressed = true;
    }

    public void OnAttack(InputAction.CallbackContext ctx) {
        if (ctx.started) {
            attackPressed = true;
            _attackBufferUntil = Time.time + attackBuffer;
        }
    }

    public void SwitchState(IPlayerState next)
    {
        _current?.Exit();
        _current = next;
        _current.Enter(this);
    }

    static Vector2 ApplyRadialDeadzone(Vector2 v, float dz)
    {
        float m = v.magnitude;
        if (m <= dz) return Vector2.zero;
        float scaled = Mathf.InverseLerp(dz, 1f, m);
        return v.normalized * scaled;
    }

    public bool ConsumeJumpBufferIfAvailable()
    {
        if (lastPressedJumpTime > 0f && lastOnGroundTime > 0f)
        {
            lastPressedJumpTime = 0f;
            lastOnGroundTime = 0f;
            return true;
        }
        return false;
    }

    public bool CanQueueJumpNow()
    {
        return IsGrounded || lastOnGroundTime > 0f || airJumpsLeft > 0;
    }

    public bool FacingRight {
        get
        {
            if (sr) return !sr.flipX;
            if (Mathf.Abs(moveInput.x) > 0.01f) return moveInput.x > 0f;
            return rb.linearVelocity.x >= 0f;
        }
    }
}