using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraDashBlend : MonoBehaviour
{
    public PlayerController player;
    public Rigidbody2D playerRb;
    public float speedThreshold = 12f;

    [Header("Normal")]
    public float normalXDamping = 0.12f;
    public float normalLookaheadTime = 0.18f;
    public float normalLookaheadSmoothing = 0.40f;
    public Vector2 normalTrackedOffset = new Vector2(0f, 0f);

    [Header("Dash")]
    public float dashXDamping = 0.00f;
    public float dashLookaheadTime = 0.40f;
    public float dashLookaheadSmoothing = 0.25f;
    public float dashLeadX = 1.0f;
    public float blendIn = 0.08f;
    public float hold = 0.10f;
    public float blendOut = 0.18f;

    CinemachineVirtualCamera _vcam;
    CinemachineFramingTransposer _body;

    enum Phase { Normal, BlendingIn, Holding, BlendingOut }
    Phase _phase = Phase.Normal;
    float _t = 0f;
    Vector2 _fromOffset, _toOffset;
    float _fromXDamp, _toXDamp, _fromLA, _toLA, _fromLAS, _toLAS;

    void Awake()
    {
        _vcam = GetComponent<CinemachineVirtualCamera>();
        _body = _vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (playerRb == null && player != null) playerRb = player.rb;

        // seed to normal
        ApplyDirect(normalXDamping, normalLookaheadTime, normalLookaheadSmoothing, normalTrackedOffset);
    }

    void Update()
    {
        bool isDashing = player != null && player.justDashed; // set true in DashState Exit; auto-resets below
        bool fastEnough = playerRb != null && Mathf.Abs(playerRb.linearVelocity.x) >= speedThreshold;

        if (_phase == Phase.Normal && (isDashing || fastEnough))
            BeginBlendIn();

        switch (_phase)
        {
            case Phase.BlendingIn:
                _t += Time.deltaTime / Mathf.Max(0.0001f, blendIn);
                LerpParams(_t);
                if (_t >= 1f) BeginHold();
                break;

            case Phase.Holding:
                _t += Time.deltaTime;
                if (_t >= hold) BeginBlendOut();
                break;

            case Phase.BlendingOut:
                _t += Time.deltaTime / Mathf.Max(0.0001f, blendOut);
                LerpParams(_t);
                if (_t >= 1f) BackToNormal();
                break;
        }

        if (player != null) player.justDashed = false; // consume the one-frame flag
    }

    void BeginBlendIn()
    {
        _phase = Phase.BlendingIn;
        _t = 0f;

        float dir = (player != null && player.FacingRight) ? 1f : -1f;

        _fromXDamp = _body.m_XDamping;
        _toXDamp = dashXDamping;

        _fromLA = _body.m_LookaheadTime;
        _toLA = dashLookaheadTime;

        _fromLAS = _body.m_LookaheadSmoothing;
        _toLAS = dashLookaheadSmoothing;

        _fromOffset = _body.m_TrackedObjectOffset;
        _toOffset = new Vector2(dashLeadX * dir, _fromOffset.y);
    }

    void BeginHold()
    {
        _phase = Phase.Holding;
        _t = 0f;
        ApplyDirect(_toXDamp, _toLA, _toLAS, _toOffset);
    }

    void BeginBlendOut()
    {
        _phase = Phase.BlendingOut;
        _t = 0f;

        _fromXDamp = _body.m_XDamping;
        _toXDamp = normalXDamping;

        _fromLA = _body.m_LookaheadTime;
        _toLA = normalLookaheadTime;

        _fromLAS = _body.m_LookaheadSmoothing;
        _toLAS = normalLookaheadSmoothing;

        _fromOffset = _body.m_TrackedObjectOffset;
        _toOffset = normalTrackedOffset;
    }

    void BackToNormal()
    {
        _phase = Phase.Normal;
        ApplyDirect(normalXDamping, normalLookaheadTime, normalLookaheadSmoothing, normalTrackedOffset);
    }

    void LerpParams(float t01)
    {
        t01 = Mathf.Clamp01(t01);
        _body.m_XDamping = Mathf.Lerp(_fromXDamp, _toXDamp, t01);
        _body.m_LookaheadTime = Mathf.Lerp(_fromLA, _toLA, t01);
        _body.m_LookaheadSmoothing = Mathf.Lerp(_fromLAS, _toLAS, t01);
        _body.m_TrackedObjectOffset = Vector2.Lerp(_fromOffset, _toOffset, t01);
    }

    void ApplyDirect(float xDamp, float la, float las, Vector2 off)
    {
        _body.m_XDamping = xDamp;
        _body.m_LookaheadTime = la;
        _body.m_LookaheadSmoothing = las;
        _body.m_TrackedObjectOffset = off;
    }
}
