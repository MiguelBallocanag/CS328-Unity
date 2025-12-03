using UnityEngine;

public class FountainShooter : MonoBehaviour
{
    [Header("Fountain Shooter Settings")]
    public GameObject energyBallPrefab;
    public Transform shootPoint;
    public Vector2 shootDirection = Vector2.right;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void ShootFromAnimations() 
    {
        GameObject energyBall = Instantiate(energyBallPrefab, shootPoint.position, Quaternion.identity);
        EnergyBall energyBallScript = energyBall.GetComponent<EnergyBall>();
        if (energyBallScript != null)
        {
            energyBallScript.Init(shootDirection);
        }


    }
}
