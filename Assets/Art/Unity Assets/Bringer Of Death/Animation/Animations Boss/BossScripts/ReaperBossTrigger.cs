using UnityEngine;
using System.Collections;

public class ReaperBossTrigger : MonoBehaviour
{
    public GameObject gate;
    public Boss_Health Boss;
    public float delayTime = 2f; 

    private bool triggered = false;
    
    // FIXED: Changed from OnTriggerExit2D to OnTriggerEnter2D
    // Boss fight now starts when player ENTERS the zone (not exits)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            
            // Close the gate behind player
            if (gate != null)
            {
                gate.SetActive(true);
            }
            
            // Start boss fight after delay
            StartCoroutine(StartBoss());
        }
    }
    
    private IEnumerator StartBoss()
    {
        yield return new WaitForSeconds(delayTime);
        
        if (Boss != null)
        {
            Boss.BossActive();
        }
        else
        {
            Debug.LogError("[ReaperBossTrigger] Boss reference is null! Assign Boss_Health component in inspector.");
        }
    }
}