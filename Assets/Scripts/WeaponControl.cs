using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponControl : MonoBehaviour
{
    [SerializeField] private Animator animator; // Animator component
    [SerializeField] private AudioSource audioSource1; // Audiosource component
    [SerializeField] private AudioClip pistolShoot; // Pistol shot audio clip

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) // Left mouse button down for new input system
        {
            animator.SetTrigger("AnimPlay"); // Sets the trigger "AnimPlay" on the pistol animator
            audioSource1.clip = pistolShoot; // Sets the audio clip of audioSource1 to the pistol shot sound effect
            audioSource1.Play(); // Plays the clip
        }
    }
}
