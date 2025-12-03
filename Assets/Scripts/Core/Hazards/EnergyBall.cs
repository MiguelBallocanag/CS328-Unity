using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 direction = Vector2.right;
    public float lifetime = 3f;
    public int damage = 2;

    // Initialize the energy ball with a direction
    public void Init(Vector2 dir)
    {
        // Normalize the direction to ensure consistent speed
        direction = dir.normalized;
        Destroy(gameObject, lifetime);

    }

    // Update is called once per frame
    void Update()
    {
        // Move the energy ball in the specified direction
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

    }
    // Handle collision with player or obstacles
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the energy ball hits the player
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Obstacle") || other.CompareTag("Enemy")) // Destroy on hitting obstacles or enemies
        {
            Destroy(gameObject);
        }
    }
}
