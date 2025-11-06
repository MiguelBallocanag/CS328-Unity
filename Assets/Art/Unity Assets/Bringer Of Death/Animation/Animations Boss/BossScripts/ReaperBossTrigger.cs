using UnityEngine;
using System.Collections;

public class ReaperBossTrigger : MonoBehaviour
{
    public GameObject gate;
    public Boss_Health Boss;
    public float delayTime = 2f; 

    private bool triggered = false;
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            if (gate != null)
            {
                gate.SetActive(true);
            }
            StartCoroutine(StartBoss());
        }
    }
    private IEnumerator StartBoss()
    {
        yield return new WaitForSeconds(delayTime);
        Boss.BossActive();
    }

}
