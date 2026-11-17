using System;
using UnityEngine;

// SUBJECT of the Observer pattern.
// It raises events when something happens, and does not know who is listening.
public class PlayerHealth : MonoBehaviour
{
    public int health = 100;

    // Events observers can subscribe to
    public event Action<int> OnDamaged;   // pushes the new health value
    public event Action OnDied;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            DecreaseHealth(10);
        }
    }

    private void DecreaseHealth(int decreaseAmount)
    {
        health -= decreaseAmount;

        OnDamaged?.Invoke(health);   // notify all observers

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        OnDied?.Invoke();            // notify all observers
    }
}