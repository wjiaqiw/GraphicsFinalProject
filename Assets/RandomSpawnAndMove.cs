using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnAndMove : MonoBehaviour
{
    public Transform target; // Target object (e.g., player)
    public float spawnRadius = 7f; // Initial spawn radius
    public float initialSpeed = 2f; // Initial movement speed
    public float initialSpawnInterval = 5f; // Initial time between spawns
    public float speedIncreaseRate = 0.1f; // Speed increase per spawn
    public float spawnRateIncrease = 0.9f; // Spawn interval multiplier (decreases over time)
    public float minSpawnInterval = 1f; // Minimum spawn interval limit
    public float rotationSpeed = 3f; // Rotation speed toward the target
    public static int activeDragonCount = 0; // Track the number of active dragons
    public static int maxDragons = 6; // Maximum dragons allowed at once

    private float currentSpeed;
    private float currentSpawnInterval;
    private bool isDead = false;

    public AudioSource audioSource;   // Reference your AudioSource here (drag in Inspector)
    public AudioClip dame; 

    // Reference to the Animator (on the parent branch)
    private Animator animator;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned!");
            return;
        }

        // Get the Animator from the parent branch.
        animator = GetComponentInParent<Animator>();

        currentSpeed = initialSpeed;
        currentSpawnInterval = initialSpawnInterval;

        if (activeDragonCount < maxDragons)
        {
            activeDragonCount++;
            Invoke("TrySpawnDragon", currentSpawnInterval);
        }
    }

    void TrySpawnDragon()
    {
        if (activeDragonCount >= maxDragons) return; // Prevent spawning more than allowed
        SpawnDragon();
    }

    void SpawnDragon()
    {
        Vector3 spawnPosition = GetRandomPositionAroundTarget();

        // Instantiate a new dragon (clone of this GameObject)
        GameObject newDragon = Instantiate(gameObject, spawnPosition, Quaternion.identity);

        // Enable the script on the clone and set its movement speed.
        RandomSpawnAndMove newScript = newDragon.GetComponent<RandomSpawnAndMove>();
        newScript.enabled = true;
        newScript.SetSpeed(currentSpeed);

        activeDragonCount++;

        currentSpeed += speedIncreaseRate;
        currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval * spawnRateIncrease);

        Invoke("TrySpawnDragon", currentSpawnInterval);
    }

    public void SetSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
    }

    Vector3 GetRandomPositionAroundTarget()
    {
        float angle = Random.Range(0f, 360f);
        float distance = Random.Range(1f, spawnRadius);
        Vector3 randomOffset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        return target.position + new Vector3 (7, 0, 7) + randomOffset;
    }

    void Update()
    {
        if (target != null && !isDead)
        {
            // Move toward the target.
            transform.position = Vector3.MoveTowards(transform.position, target.position, currentSpeed * Time.deltaTime);
            LookAtTargetSmoothly();
        }
    }

    void LookAtTargetSmoothly()
    {
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0;

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // Call this method when the dragon should die
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        activeDragonCount--; // Reduce count when a dragon dies

        // If the animator is not already referenced, try to get it again.
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (animator != null)
        {
            // Set the bool parameter to true to trigger the death state
            animator.SetBool("isDead", true);
        }

        // Destroy the object after a delay (adjust 3f to match your death animation length)
        if (audioSource != null && dame != null)
        {
            audioSource.PlayOneShot(dame);
        }
        Destroy(gameObject, 1.8f);
    }

    // Example collision detection for a projectile hit.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Die();
        }
    }
}
