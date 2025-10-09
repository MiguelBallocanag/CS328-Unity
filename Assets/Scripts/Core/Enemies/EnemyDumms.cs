using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDummy : MonoBehaviour, IDamageable {
    public int hp = 30;
    Rigidbody2D rb;
    void Awake(){ rb = GetComponent<Rigidbody2D>(); }

    public void TakeHit(DamageContext ctx) {
        hp -= ctx.damage;
        rb.linearVelocity = ctx.knockback; // small knockback
        // optional: flash, play sfx
        if (hp <= 0) Destroy(gameObject);
    }
}
