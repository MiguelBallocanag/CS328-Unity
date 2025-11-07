using UnityEngine;
using Cinemachine;

public class GroundedLock : CinemachineExtension
{
    public Rigidbody2D rb;
    public Transform groundCheck;
    public float groundRadius = 0.15f;
    public float yOffsetCam = 0.25f;

    public LayerMask groundMask;

    public float coyoteTime = 0.12f;
    private float lastGroundedTime;
    private float lockedY;

    protected override void OnEnable()
    {
        base.OnEnable();
        lastGroundedTime = 0f;
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Body) return;

        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);

        if (grounded)
        {
            lastGroundedTime = Time.time;
            lockedY = rb.position.y;
        }

        // apply lock while grounded + short time after jumping (coyote feel)
        if (Time.time - lastGroundedTime < coyoteTime)
        {
            Vector3 pos = state.RawPosition;
            pos.y = lockedY + yOffsetCam;

            state.RawPosition = pos;
        }
    }
}
