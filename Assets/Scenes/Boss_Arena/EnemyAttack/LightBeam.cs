using UnityEngine;
using System.Collections;

public class LightBeam : MonoBehaviour
{
    [Header("Child Objects")]
    public GameObject warningBeam;  // Yellow transparent beam
    public GameObject damageBeam;   // White damage beam
    
    [Header("Settings")]
    public int damage = 15;
    
    private float warningDuration;
    private float damageDuration;
    private bool isActive = false;
    private bool hasDealtDamage = false; // Prevent multiple damage per beam
    
    void Start()
    {
        // Make sure damage beam is initially disabled
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
        // Phase 1: Warning (yellow beam, no damage)
        if (warningBeam != null)
        {
            warningBeam.SetActive(true);
        }
        
        Debug.Log("[LightBeam] Warning phase...");
        yield return new WaitForSeconds(warningDuration);
        
        // Phase 2: Damage (white beam, deals damage)
        if (warningBeam != null)
        {
            warningBeam.SetActive(false);
        }
        
        if (damageBeam != null)
        {
            damageBeam.SetActive(true);
        }
        
        isActive = true;
        Debug.Log("[LightBeam] DAMAGE ACTIVE!");
        
        // Optional: Play beam sound
        // AudioManager.Instance?.PlayLightBeam();
        
        yield return new WaitForSeconds(damageDuration);
        
        // Cleanup
        isActive = false;
        Destroy(gameObject);
    }
    
    // Deal damage once when player enters beam
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || hasDealtDamage) return;
        
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[LightBeam] Player entered beam! Damage: {damage}");
            
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                hasDealtDamage = true; // Only damage once per beam activation
            }
        }
    }
}