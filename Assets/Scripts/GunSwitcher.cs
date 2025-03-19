using UnityEngine;

public class GunSwitcher : MonoBehaviour
{
    public GameObject[] guns; // Assign all gun GameObjects in the Inspector
    private int currentGunIndex = 0; // Tracks the current gun

    void Start()
    {
        // Ensure only one gun is active at the start
        ActivateGun(currentGunIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Press 'Q' to switch guns
        {
            SwitchGun();
        }
    }

    void SwitchGun()
    {
        // Disable the current gun
        guns[currentGunIndex].SetActive(false);

        // Move to the next gun in the list
        currentGunIndex = (currentGunIndex + 1) % guns.Length;

        // Enable the new gun
        guns[currentGunIndex].SetActive(true);
    }

    void ActivateGun(int index)
    {
        // Disable all guns
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].SetActive(i == index); // Only enable the selected gun
        }
    }
}
