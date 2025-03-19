using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;  // Reference to the bullet prefab
    public Transform firePoint;      // Where bullets are spawned
    public float bulletForce = 20f;  // Bullet speed
    public float fireRate = 0.5f;    // Fire rate
    private float nextFireTime = 0f; // Timer for firing

    [Header("Bobbing Settings")]
    public float bobAmplitude = 0.05f;  // How far the gun moves up/down
    public float bobFrequency = 5f;     // How quickly the gun bobs
    public float bobSmoothing = 6f;     // How smoothly it transitions

    [Header("Recoil Settings")]
    public float recoilAmount = 0.1f;   // How far the gun moves back on recoil
    public float recoilSpeed = 6f;      // How quickly the gun recovers

    private Vector3 initialLocalPos;    // Gun's local starting position
    private float bobTimer;             // Timer for bobbing
    private bool isRecoiling;           // Tracks recoil state
    private float recoilTimer;          // Timer for recoil interpolation

    void Start()
    {
        initialLocalPos = transform.localPosition;
    }

    void Update()
    {
        HandleBobbing();
        HandleRecoil();

        // Fire logic
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            StartRecoil();  // Trigger recoil when shooting
            nextFireTime = Time.time + fireRate;
        }
    }

    /// <summary>
    /// Spawns a bullet and applies force.
    /// </summary>
    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Applies a simple up/down bobbing motion when moving.
    /// </summary>
    void HandleBobbing()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // If the player is moving, advance bob timer; otherwise, smoothly reset it.
        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            bobTimer += Time.deltaTime * bobFrequency;
        }
        else
        {
            bobTimer = Mathf.Lerp(bobTimer, 0f, Time.deltaTime * bobSmoothing);
        }

        // Calculate bob offset using a sine wave.
        float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;
        Vector3 targetPosition = initialLocalPos + new Vector3(0f, bobOffset, 0f);

        // Smoothly transition the gun's local position to the bob offset.
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * bobSmoothing);
    }

    /// <summary>
    /// Starts the recoil process when a shot is fired.
    /// </summary>
    void StartRecoil()
    {
        isRecoiling = true;
        recoilTimer = 0f;
    }

    /// <summary>
    /// Moves the gun backward briefly on recoil, then smoothly returns it.
    /// </summary>
    void HandleRecoil()
    {
        if (!isRecoiling) return;

        recoilTimer += Time.deltaTime * recoilSpeed;
        float recoilFactor = Mathf.Clamp01(recoilTimer);

        // From localPos + offset to localPos over time:
        Vector3 recoilOffset = new Vector3(0, 0, -recoilAmount);
        Vector3 targetRecoilPos = Vector3.Lerp(initialLocalPos + recoilOffset, initialLocalPos, recoilFactor);

        transform.localPosition = targetRecoilPos;

        // Once the interpolation reaches 1, recoil is complete.
        if (recoilFactor >= 1f)
        {
            isRecoiling = false;
        }
    }
}

