using UnityEngine;
using TMPro;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3; // Time before bullet disappears

    void Awake()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the object we hit has the "Enemy" tag.
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Get the enemy's controlling script from the parent branch.
            RandomSpawnAndMove enemyController = collision.gameObject.GetComponentInParent<RandomSpawnAndMove>();
            if (enemyController != null)
            {
                enemyController.Die();
            }
            
            // Optionally, destroy the bullet after it has hit.
            Destroy(gameObject);
        }
    }
}
