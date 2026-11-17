using UnityEngine;

// RECEIVER of the Command pattern: knows how to do the work,
// but no longer reads input itself.
public class PlayerShooting : MonoBehaviour
{
    public Gun gun;
    public Transform gunHolder;
    private bool isHoldingShoot = false;

    // Called by StartShootCommand
    public void StartShooting()
    {
        isHoldingShoot = true;
    }

    // Called by StopShootCommand
    public void StopShooting()
    {
        isHoldingShoot = false;
    }

    // Called by ReloadCommand
    public void Reload()
    {
        if (gun != null)
        {
            gun.TryReload();
        }
    }

    void Update()
    {
        if (isHoldingShoot && gun != null)
        {
            gun.Shoot();
        }
    }

    public void OnDrop()
    {
        if (gun != null)
        {
            gun.Drop();
            gun = null;
        }
    }
}