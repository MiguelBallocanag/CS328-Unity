using UnityEngine;
using System.Collections;

public class BulletHellBoss : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject phase1Sprite;
    public GameObject phase2Sprite;
    public Animator animator;
    
    [Header("Phase Transition")]
    public float transitionDuration = 2f;
    public GameObject transitionEffect;
    
    private BulletHellBoss_Phase1 phase1Controller;
    private BulletHellBoss_Phase2 phase2Controller;
    private bool isInPhase2 = false;
    
    void Start()
    {
        phase1Controller = GetComponent<BulletHellBoss_Phase1>();
        phase2Controller = GetComponent<BulletHellBoss_Phase2>();
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
        
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }
        
        if (phase1Sprite != null) phase1Sprite.SetActive(true);
        if (phase2Sprite != null) phase2Sprite.SetActive(false);
        
        // Disable both phases on start - trigger will activate Phase1
        if (phase1Controller != null) phase1Controller.enabled = false;
        if (phase2Controller != null) phase2Controller.enabled = false;
    }
    
    public void ActivateBoss()
    {
        Debug.Log("[BulletHellBoss] ActivateBoss() called!");
        
        if (phase1Controller != null)
        {
            phase1Controller.enabled = true;
            phase1Controller.StartPhase1();
            Debug.Log("[BulletHellBoss] Phase1 enabled and started!");
        }
        else
        {
            Debug.LogError("[BulletHellBoss] Phase1 controller is NULL!");
        }
    }
    
    public void TriggerPhase2Transition()
    {
        if (isInPhase2) return;
        StartCoroutine(TransitionToPhase2());
    }
    
    IEnumerator TransitionToPhase2()
    {
        if (phase1Controller != null)
        {
            phase1Controller.StopAttacking();
            phase1Controller.enabled = false;
        }
        
        if (animator != null)
        {
            animator.SetTrigger("PhaseTransition");
            animator.SetBool("IsPhase2", true);
        }
        
        if (transitionEffect != null) Instantiate(transitionEffect, transform.position, Quaternion.identity);
        
        yield return new WaitForSeconds(transitionDuration);
        
        if (phase1Sprite != null) phase1Sprite.SetActive(false);
        if (phase2Sprite != null) phase2Sprite.SetActive(true);
        
        if (phase2Controller != null)
        {
            phase2Controller.enabled = true;
            phase2Controller.StartPhase2();
        }
        
        isInPhase2 = true;
    }
}