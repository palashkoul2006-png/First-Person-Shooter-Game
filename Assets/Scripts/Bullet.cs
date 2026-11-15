using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 15f;
    public float LifeTime = 3f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = -transform.right * Speed;
        Destroy(gameObject, LifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
