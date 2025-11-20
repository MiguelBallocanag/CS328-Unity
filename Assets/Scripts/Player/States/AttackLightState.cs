using System.Collections.Generic;
using UnityEngine;

public class AttackLightState : AirState {
    // data for this swing (ScriptableObject)
    private readonly AttackData data;

    // phase timers
    private float timer;
    private float startupT, activeT, recoveryT;

    // prevent multi-hits on the same target during one swing
    private static readonly Collider2D[] _hitsBuf = new Collider2D[8];
    private readonly HashSet<Collider2D> _alreadyHit = new HashSet<Collider2D>(8);

    // config: lock ground X during attack (air keeps momentum)
    private readonly bool lockGroundX = false;

    public AttackLightState(AttackData d) { data = d; }

    public override void Enter(PlayerController p) {
        base.Enter(p);

        // animation trigger
        pc.TryStartAttackAnim();

        // cache timings
        startupT  = data.startup;
        activeT   = data.active;
        recoveryT = data.recovery;
        timer     = 0f;

        _alreadyHit.Clear();

        // optional: stop ground sliding on startup for crisp feel
        if (pc.IsGrounded && lockGroundX) {
            pc.rb.linearVelocity = new Vector2(0f, pc.rb.linearVelocity.y);
        }
    }

    public override void Tick() {
        // FIXED: Use fixedDeltaTime for frame-rate independent physics
        float dt = Time.fixedDeltaTime;
        timer += dt;

        // light locomotion while attacking
        if (pc.IsGrounded) {
            if (!lockGroundX) ApplyGroundControlLight();
        } else {
            ApplyAirControlLight();
            ApplyBaseGravity();
            ApplyEarlyCutIfNeeded();
            ApplyHeavierFall();
        }

        // phases
        if (timer < startupT) return; // startup: no hitbox

        // allow jump-cancel during active/recovery (ground-only)
        if (pc.jumpPressed && pc.lastOnGroundTime > 0f) {
            pc.SwitchState(new JumpState());
            return;
        }

        if (timer < startupT + activeT) { // active: can hit
            DoHitbox();
            return;
        }

        // recovery: allow chaining
        if (timer < startupT + activeT + recoveryT) {
            if ((pc.attackPressed || pc.AttackBuffered) && pc.lightAttack != null) {
                pc.ConsumeAttackBuffer();
                pc.SwitchState(new AttackLightState(pc.lightAttack));
                return;
            }
        }

        // recovery done: leave state
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

        // compute world-space box center
        Vector2 local = data.localOffset;
        if (!facingRight) local.x = -local.x;

        Vector2 center = (Vector2)pc.transform.position + local;

        // contact filter for target layers
        var filter = new ContactFilter2D {
            useLayerMask = true,
            layerMask = data.targets
        };

        int count = Physics2D.OverlapBox(center, data.boxSize, 0f, filter, _hitsBuf);

        for (int i = 0; i < count; i++) {
            var col = _hitsBuf[i];
            if (!col || _alreadyHit.Contains(col)) continue;

            // build damage context (flip knockback by facing)
            Vector2 kb = data.knockback;
            if (!facingRight) kb.x = -kb.x;

            var ctx = new DamageContext(data.damage, data.hitstun, kb, center);

            // prefer IDamageable
            if (col.TryGetComponent<IDamageable>(out var dmg)) {
                dmg.TakeHit(ctx);
                _alreadyHit.Add(col);
                if (_alreadyHit.Count >= data.maxHitsPerSwing) break;
                continue;
            }

            // fallback: push rigidbody if no IDamageable
            var rb2d = col.attachedRigidbody;
            if (rb2d) {
                rb2d.linearVelocity = kb;
                _alreadyHit.Add(col);
                if (_alreadyHit.Count >= data.maxHitsPerSwing) break;
            }
        }
    }

    // ——— light control helpers (keep attack feel responsive) ———

    private void ApplyAirControlLight() {
        float x = pc.moveInput.x;
        float vx = pc.rb.linearVelocity.x;
        float cap = Mathf.Max(pc.runSpeed, pc.maxAirSpeed);
        float target = x * cap;
        // FIXED: Use fixedDeltaTime for consistent physics
        float accel = (Mathf.Abs(x) > 0.01f ? pc.airAccel : pc.airDecel) * Time.fixedDeltaTime;
        float newVx = Mathf.MoveTowards(vx, target, accel);
        pc.rb.linearVelocity = new Vector2(newVx, pc.rb.linearVelocity.y);
    }

    private void ApplyGroundControlLight() {
        float x = pc.moveInput.x;
        float target = x * pc.runSpeed;
        // FIXED: Use fixedDeltaTime for consistent physics
        float accel  = (Mathf.Abs(x) > 0.01f ? pc.groundAccel : pc.groundDecel) * Time.fixedDeltaTime;
        float newVx  = Mathf.MoveTowards(pc.rb.linearVelocity.x, target, accel);
        pc.rb.linearVelocity = new Vector2(newVx, pc.rb.linearVelocity.y);
    }

    public override void Exit() { }
}
