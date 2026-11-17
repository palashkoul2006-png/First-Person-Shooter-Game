using UnityEngine;

// OBSERVER of PlayerHealth: plays the hit sound when the player is damaged.
// This is a brand new observer, added WITHOUT editing PlayerHealth.
public class PlayerAudioObserver : MonoBehaviour
{
    public AudioClip hitSFX;

    private PlayerHealth playerHealth;

    private void OnEnable()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.OnDamaged += OnPlayerDamaged;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnDamaged -= OnPlayerDamaged;
    }

    private void OnPlayerDamaged(int health)
    {
        if (hitSFX != null)
            AudioManager.instance.PlaySFX(hitSFX);
    }
}