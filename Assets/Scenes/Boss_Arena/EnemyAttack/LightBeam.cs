using UnityEngine;
using System.Collections;

public class LightBeam : MonoBehaviour
{
    [Header("Child Objects")]
    public GameObject warningBeam;
    public GameObject damageBeam;
    
    [Header("Settings")]
    public int damage = 15;
    public Vector2 beamSize = new Vector2(1f, 10f);
    
    [Header("Knockback")]
    public float launchForce = 15f; // How hard to launch player up
    
    private float warningDuration;
    private float damageDuration;
    private bool isActive = false;
    private bool hasDealtDamage = false;
    
    void Start()
    {
        if (damageBeam != null)
        {
            damageBeam.SetActive(false);
        }
    }
    
    public void Initialize(float warningTime, float damageTime)
    {
        warningDuration = warningTime;
        damageDuration = damageTime;
        StartCoroutine(BeamSequence());
    }
    
    IEnumerator BeamSequence()
    {
        if (warningBeam != null)
        {
            warningBeam.SetActive(true);
        }
        
        yield return new WaitForSeconds(warningDuration);
        
        if (warningBeam != null)
        {
            warningBeam.SetActive(false);
        }
        
        if (damageBeam != null)
        {
            damageBeam.SetActive(true);
        }
        
        isActive = true;
        hasDealtDamage = false;
        
        yield return new WaitForSeconds(damageDuration);
        
        isActive = false;
        Destroy(gameObject);
    }
    
    void Update()
    {
        if (isActive && !hasDealtDamage)
        {
            CheckForPlayer();
        }
    }
    
    void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, beamSize, 0f);
        
        foreach (Collider2D hit in hits)
        {
            PlayerHealth playerHealth = hit.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log($"[LightBeam] HIT PLAYER! Damage: {damage}");
                playerHealth.TakeDamage(damage);
                
                // LAUNCH PLAYER INTO THE AIR!
                Rigidbody2D playerRb = hit.GetComponentInParent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, launchForce);
                }
                
                hasDealtDamage = true;
                return;
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, beamSize);
    }
}
