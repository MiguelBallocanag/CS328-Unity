using System.Collections.Generic;
using UnityEngine;

public class AttackLightState : AirState {
    private readonly AttackData data;
    private float timer;
    private float startupT, activeT, recoveryT;
    private static readonly Collider2D[] _hitsBuf = new Collider2D[8];
    private readonly HashSet<Collider2D> _alreadyHit = new HashSet<Collider2D>(8);
    private readonly bool lockGroundX = false;

    // CRITICAL: Track when we entered active phase
    private int activePhaseFrameCount = 0;
    private bool inActivePhase = false;

    public AttackLightState(AttackData d) { data = d; }

    public override void Enter(PlayerController p) {
        base.Enter(p);
        pc.TryStartAttackAnim();

        startupT  = data.startup;
        activeT   = data.active;
        recoveryT = data.recovery;
        timer     = 0f;

        _alreadyHit.Clear();
        activePhaseFrameCount = 0;
        inActivePhase = false;

        if (pc.IsGrounded && lockGroundX) {
            pc.rb.linearVelocity = new Vector2(0f, pc.rb.linearVelocity.y);
        }
    }

    public override void Tick() {
        float dt = Time.fixedDeltaTime;
        timer += dt;

        // Movement
        if (pc.IsGrounded) {
            if (!lockGroundX) ApplyGroundControlLight();
        } else {
            ApplyAirControlLight();
            ApplyBaseGravity();
            ApplyEarlyCutIfNeeded();
            ApplyHeavierFall();
        }

        // STARTUP PHASE
        if (timer < startupT) {
            inActivePhase = false;
            return;
        }

        // Jump cancel
        if (pc.jumpPressed && pc.lastOnGroundTime > 0f) {
            pc.SwitchState(new JumpState());
            return;
        }

        // ACTIVE PHASE - Only check hitbox on the FIRST frame we enter this phase
        if (timer >= startupT && timer < startupT + activeT) {
            if (!inActivePhase) {
                // This is the FIRST frame of active phase
                inActivePhase = true;
                activePhaseFrameCount = 0;
                DoHitbox(); // Check ONCE
                Debug.Log("=== ACTIVE PHASE: Checked hitbox ONCE ===");
            }
            activePhaseFrameCount++;
            return;
        }

        // RECOVERY PHASE
        if (timer < startupT + activeT + recoveryT) {
            inActivePhase = false;
            if ((pc.attackPressed || pc.AttackBuffered) && pc.lightAttack != null) {
                pc.ConsumeAttackBuffer();
                pc.SwitchState(new AttackLightState(pc.lightAttack));
                return;
            }
        }

        // DONE
        if (timer >= startupT + activeT + recoveryT) {
            if (pc.IsGrounded) {
                if (Mathf.Abs(pc.moveInput.x) < 0.1f) pc.SwitchState(new IdleState());
                else pc.SwitchState(new RunState());
            } else {
                if (pc.rb.linearVelocity.y > 0f) pc.SwitchState(new JumpState());
                else pc.SwitchState(new FallState());
            }
        }
    }

    private void DoHitbox() {
        bool facingRight = pc.FacingRight; 
        Vector2 local = data.localOffset;
        if (!facingRight) local.x = -local.x;
        Vector2 center = (Vector2)pc.transform.position + local;

        var filter = new ContactFilter2D {
            useLayerMask = true,
            layerMask = data.targets
        };

        int count = Physics2D.OverlapBox(center, data.boxSize, 0f, filter, _hitsBuf);
        Debug.Log($"DoHitbox found {count} potential targets");

        for (int i = 0; i < count; i++) {
            var col = _hitsBuf[i];
            if (!col || _alreadyHit.Contains(col)) continue;

            Vector2 kb = data.knockback;
            if (!facingRight) kb.x = -kb.x;
            var ctx = new DamageContext(data.damage, data.hitstun, kb, center);

            if (col.TryGetComponent<IDamageable>(out var dmg)) {
                Debug.Log($">>> Calling TakeHit on {col.gameObject.name}");
                dmg.TakeHit(ctx);
                _alreadyHit.Add(col);
                if (_alreadyHit.Count >= data.maxHitsPerSwing) break;
                continue;
            }

            var rb2d = col.attachedRigidbody;
            if (rb2d) {
                rb2d.linearVelocity = kb;
                _alreadyHit.Add(col);
                if (_alreadyHit.Count >= data.maxHitsPerSwing) break;
            }
        }
    }

    private void ApplyAirControlLight() {
        float x = pc.moveInput.x;
        float vx = pc.rb.linearVelocity.x;
        float cap = Mathf.Max(pc.runSpeed, pc.maxAirSpeed);
        float target = x * cap;
        float accel = (Mathf.Abs(x) > 0.01f ? pc.airAccel : pc.airDecel) * Time.fixedDeltaTime;
        float newVx = Mathf.MoveTowards(vx, target, accel);
        pc.rb.linearVelocity = new Vector2(newVx, pc.rb.linearVelocity.y);
    }

    private void ApplyGroundControlLight() {
        float x = pc.moveInput.x;
        float target = x * pc.runSpeed;
        float accel  = (Mathf.Abs(x) > 0.01f ? pc.groundAccel : pc.groundDecel) * Time.fixedDeltaTime;
        float newVx  = Mathf.MoveTowards(pc.rb.linearVelocity.x, target, accel);
        pc.rb.linearVelocity = new Vector2(newVx, pc.rb.linearVelocity.y);
    }

    public override void Exit() { }
}