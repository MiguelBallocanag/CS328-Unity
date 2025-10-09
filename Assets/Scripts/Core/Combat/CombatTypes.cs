using UnityEngine;

public struct DamageContext {
    public int damage;
    public float hitstun;
    public Vector2 knockback;
    public Vector2 hitOrigin;

    public DamageContext(int dmg, float stun, Vector2 kb, Vector2 origin) {
        damage = dmg; hitstun = stun; knockback = kb; hitOrigin = origin;
    }
}

public interface IDamageable {
    void TakeHit(DamageContext ctx);
}
