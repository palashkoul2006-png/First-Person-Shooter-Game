using UnityEngine;
using UnityEngine.InputSystem;

// OBSERVER of PlayerHealth: shakes the camera when the player is damaged.
public class PlayerLook : MonoBehaviour
{
    public static PlayerLook Instance;
    public float mouseSensitivity = 200f;
    public Transform cam;

    private float xRotation = 0f;
    private Vector2 lookInput;

    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private float shakeFadeSpeed = 1.5f;
    private Vector3 initialCamPos;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        // Subscribe to the subject's event
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.OnDamaged += OnPlayerDamaged;
    }

    private void OnDisable()
    {
        // Unsubscribe
        if (playerHealth != null)
            playerHealth.OnDamaged -= OnPlayerDamaged;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        initialCamPos = cam.localPosition;
    }

    void Update()
    {
        HandleMouseLook();
        HandleShake();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    // Observer callback: runs whenever PlayerHealth raises OnDamaged
    private void OnPlayerDamaged(int health)
    {
        AddShake(0.1f, 0.25f);
    }

    void HandleMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleShake()
    {
        if (shakeDuration > 0)
        {
            cam.localPosition = initialCamPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * shakeFadeSpeed;
        }
        else
        {
            cam.localPosition = initialCamPos;
        }
    }

    public void AddShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}