using UnityEngine;

/// <summary>
/// Trigger to activate the Bullet Hell Boss
/// Place this on a trigger collider at the arena entrance
/// </summary>
public class BulletHellBossTrigger : MonoBehaviour
{
    [Header("Boss Reference")]
    public BulletHellBoss boss;
    
    [Header("Settings")]
    public float activationDelay = 2f;
    
    private bool hasTriggered = false;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            Debug.Log("[BulletHellBossTrigger] Player entered arena!");
            
            if (activationDelay > 0)
            {
                Invoke(nameof(ActivateBoss), activationDelay);
            }
            else
            {
                ActivateBoss();
            }
        }
    }
    
    void ActivateBoss()
    {
        if (boss != null)
        {
            boss.ActivateBoss();
            Debug.Log("[BulletHellBossTrigger] Boss activated!");
        }
        else
        {
            Debug.LogError("[BulletHellBossTrigger] No boss assigned!");
        }
    }
}