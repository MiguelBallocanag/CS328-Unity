using UnityEngine;

public class WizardRangedAttack : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown = 2.5f;
    [SerializeField] private float attackRange = 6f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject fireballPrefab;

    private Animator anim;
    private Transform player;
    private float cooldownTimer = Mathf.Infinity;
    private bool isDead = false;

    private string paramAttack = "Fire"; // Animator trigger name

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isDead)
            return;

        if (player == null || firePoint == null || fireballPrefab == null)
            return;

        cooldownTimer += Time.deltaTime;

        if (PlayerInRange() && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0f;
            anim.SetTrigger(paramAttack);
        }
    }

    private bool PlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    private void Shoot()
    {
        if (isDead || player == null) return;

        GameObject fireball = Instantiate(
            fireballPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Fireball fb = fireball.GetComponent<Fireball>();
        if (fb != null)
        {
            float direction = player.position.x > transform.position.x ? 1f : -1f;
            fb.SetDirection(new Vector2(direction, 0f));

            if (direction < 0)
                fireball.transform.localScale = new Vector3(-1, 1, 1);
        }
    }



    // Called from EnemyHealth
    public void OnEnemyDeath()
    {
        isDead = true;
        CancelInvoke();
        this.enabled = false;
    }

    // Editor visual for range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
