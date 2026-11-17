using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// CLIENT of the Decorator pattern: reads fire rate, magazine size and
// sound volume from an IWeaponStats object that may be wrapped by decorators.
public class Gun : MonoBehaviour
{
    public float reloadTime = 1f;
    public float fireRate = 0.15f;
    public int magSize = 20;

    public AudioClip shootingSFX;

    public GameObject bullet;
    public Transform bulletSpawnPoint;

    public GameObject weaponFlash;
    public GameObject droppedWeapon;

    public float recoilDistance = 0.1f;
    public float recoilSpeed = 15f;

    // ---- Decorator pattern: attachments (toggle in play mode with keys 1, 2, 3) ----
    [Header("Attachments (keys: 1 = Silencer, 2 = Rapid Fire, 3 = Extended Mag)")]
    public bool silencer;
    public bool rapidFire;
    public bool extendedMag;
    public string activeStats;   // Inspector display only

    private const float BaseVolume = 0.25f;
    private IWeaponStats stats;

    private int currentAmmo;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private Vector3 reloadRoationOffset = new Vector3(66, 50, 50);

    void Awake()
    {
        RebuildStats();
    }

    void Start()
    {
        currentAmmo = stats.MagSize;
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
        UIManager.Instance.ammoText.text = currentAmmo.ToString();
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.digit1Key.wasPressedThisFrame) { silencer = !silencer; OnAttachmentsChanged(); }
        if (kb.digit2Key.wasPressedThisFrame) { rapidFire = !rapidFire; OnAttachmentsChanged(); }
        if (kb.digit3Key.wasPressedThisFrame) { extendedMag = !extendedMag; OnAttachmentsChanged(); }
    }

    // Wrap the base stats with one decorator per active attachment
    void RebuildStats()
    {
        IWeaponStats s = new BaseWeaponStats(fireRate, magSize, BaseVolume);

        if (silencer) s = new SilencerDecorator(s);
        if (rapidFire) s = new RapidFireDecorator(s);
        if (extendedMag) s = new ExtendedMagDecorator(s);

        stats = s;
        activeStats = stats.Description;
    }

    void OnAttachmentsChanged()
    {
        RebuildStats();

        if (currentAmmo > stats.MagSize)
            currentAmmo = stats.MagSize;

        UIManager.Instance.ammoText.text = currentAmmo.ToString();
        Debug.Log("Gun stats: " + stats.Description +
                  " | FireRate " + stats.FireRate +
                  " | Mag " + stats.MagSize +
                  " | Volume " + stats.SfxVolume);
    }

    public void Shoot()
    {
        if (isReloading) return;
        if (Time.time < nextTimeToFire) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        nextTimeToFire = Time.time + stats.FireRate;   // decorated value
        currentAmmo--;
        UIManager.Instance.ammoText.text = currentAmmo.ToString();

        AudioManager.instance.PlaySFX(shootingSFX, stats.SfxVolume);   // decorated value

        Quaternion adjustRotation = bulletSpawnPoint.rotation * Quaternion.Euler(-1f, -3f, 0f);

        Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        StopCoroutine(nameof(Recoil));
        StartCoroutine(nameof(Recoil));
    }

    IEnumerator Reload()
    {
        isReloading = true;

        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles + reloadRoationOffset);
        float halfreloadTime = reloadTime / 2f;
        float t = 0f;

        while (t < halfreloadTime)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, t / halfreloadTime);
            yield return null;
        }

        t = 0f;

        while (t < halfreloadTime)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(targetRotation, initialRotation, t / halfreloadTime);
            yield return null;
        }

        currentAmmo = stats.MagSize;   // decorated value
        UIManager.Instance.ammoText.text = currentAmmo.ToString();
        isReloading = false;
    }

    public void TryReload()
    {
        if (isReloading) return;
        if (currentAmmo >= stats.MagSize) return;

        StartCoroutine(Reload());
    }

    private IEnumerator Recoil()
    {
        Vector3 recoilTarget = initialPosition + new Vector3(0, 0, -recoilDistance);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(initialPosition, recoilTarget, t);
            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(recoilTarget, initialPosition, t);
            yield return null;
        }

        transform.localPosition = initialPosition;
    }

    public void Drop()
    {
        UIManager.Instance.ammoText.text = "";
        Instantiate(droppedWeapon, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}