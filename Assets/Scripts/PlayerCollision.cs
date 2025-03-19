using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DynamicPortals;

public class PlayerCollision : MonoBehaviour
{
    [Header("Collision Settings")]
    public int maxCollisions = 3;  // Game Over occurs when collisions exceed this value (i.e. 3 collisions for 3 hearts)
    private int collisionCount = 0;

    [Header("Knockback Settings")]
    public float knockbackForce = 30f;  // Adjust for desired pushback strength

    [Header("UI References")]
    public TMP_Text gameOverText;
    public GameObject[] hearts;  // Array holding three heart GameObjects

    public AudioSource audioSource;   // Reference your AudioSource here (drag in Inspector)
    public AudioClip Damage; 

    private Player _player;

    void Start()
    {
        // Hide the "Game Over" text initially
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        // Ensure all hearts are active at the start
        if (hearts != null)
        {
            foreach (GameObject heart in hearts)
            {
                if (heart != null)
                    heart.SetActive(true);
            }
        }

        // Cache the player's script (assuming this script is on the Player)
        _player = GetComponent<Player>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            collisionCount++;
            Debug.Log("Collision Count: " + collisionCount);

            // Apply knockback using the Player script's method
            Vector3 knockbackDir = (transform.position - other.transform.position).normalized;
            _player.ApplyKnockback(knockbackDir, knockbackForce);

            if (audioSource != null && Damage != null)
            {
                audioSource.PlayOneShot(Damage);
            }
            // Remove one heart: deactivate the corresponding heart GameObject
            if (hearts != null && collisionCount <= hearts.Length)
            {
                hearts[collisionCount - 1].SetActive(false);
            }

            // Check if we exceed max collisions -> Game Over
            if (collisionCount > maxCollisions)
            {
                GameOver();
                SceneManager.LoadScene("StartScene");
            }
        }
    }

    /// <summary>
    /// Displays Game Over text and handles end-game logic.
    /// </summary>
    void GameOver()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "Game Over!";
        }
        Debug.Log("Game Over!");
    }
}