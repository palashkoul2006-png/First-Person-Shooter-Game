using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// OBSERVER of PlayerHealth: reacts to damage and death events.
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject hitUI;

    public GameObject deathUI;

    public TextMeshProUGUI ammoText;

    public Image healthBar;
    public Gradient healthGradient;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        Time.timeScale = 1.0f;
        Instance = this;
    }

    private void Start()
    {
        // Subscribe to the subject's events
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.OnDamaged += HandleDamaged;
            playerHealth.OnDied += EnableDeathUI;
        }
    }

    private void OnDestroy()
    {
        // Always unsubscribe to avoid leaks / missing reference errors
        if (playerHealth != null)
        {
            playerHealth.OnDamaged -= HandleDamaged;
            playerHealth.OnDied -= EnableDeathUI;
        }
    }

    // Called by the event whenever the player takes damage
    private void HandleDamaged(int health)
    {
        SetHealthValue(health);
        InstantiateHitUI();
    }

    public void InstantiateHitUI()
    {
        Instantiate(hitUI, transform);
    }

    public void RestartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EnableDeathUI()
    {
        deathUI.SetActive(true);
    }

    public void SetHealthValue(int health)
    {
        float floathealth = (float)health / 100;
        healthBar.color = healthGradient.Evaluate(floathealth);
        healthBar.fillAmount = floathealth;
    }
}