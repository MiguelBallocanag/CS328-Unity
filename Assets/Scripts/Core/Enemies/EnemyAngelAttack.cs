using UnityEngine;

public class AngelAttack : MonoBehaviour
{
    [Header("Ranges")]
    public float attackRange = 8f;

    [Header("Timing")]
    public float attackCooldown = 3f;
    public float castDelay = 0.5f;

    [Header("Holy Pillar")]
    public GameObject holyPillarPrefab;

    private Transform player;
    private Animator anim;
    private float cooldownTimer = Mathf.Infinity;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead || player == null) return;

        cooldownTimer += Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange && cooldownTimer >= attackCooldown)
        {
            cooldownTimer = 0f;
            anim.SetTrigger("Cast");
            Invoke(nameof(SpawnPillar), castDelay);
        }
    }

    void SpawnPillar()
    {
        if (holyPillarPrefab == null || player == null) return;

        Vector3 spawnPos = new Vector3(
            player.position.x,
            player.position.y - 0.5f,   // ground offset tweak if needed
            0f
        );

        Instantiate(holyPillarPrefab, spawnPos, Quaternion.identity);
    }

    // Optional hook if you add AngelHealth later
    public void OnAngelDeath()
    {
        isDead = true;
        CancelInvoke();
        this.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
