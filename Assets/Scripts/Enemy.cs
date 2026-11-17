using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour   // CONTEXT of the State pattern
{
    public int health = 100;

    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public GameObject weaponFlash;
    public float bloom;
    public float fireRate;
    private float lastShotTime = 0f;

    public Material hitMat;
    public AudioClip shootingSFX;

    private Rigidbody rb;
    private Renderer rend;
    private Material originalMaterial;

    // AI settings
    public int currentPointIndex = 0;
    public Vector3 currentTarget;
    public float positionThreshhold;
    public float idleTime = 5f;
    public float attackDistance = 5f;
    public float maxVisionDistance = 20f;
    public float minChasingHealth = 30f;

    public Transform[] patrolPoints;

    // made public so the state classes can use them
    [HideInInspector] public float idleTimeCounter;
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public bool canSeePlayer;
    [HideInInspector] public Vector3 lastKnownPlayerPosition;
    [HideInInspector] public NavMeshAgent agent;

    // ---- State pattern ----
    public IEnemyState currentState;
    public string currentStateName;   // Inspector display only (interfaces can't be shown)

    public readonly IdleState idleState = new IdleState();
    public readonly PatrollingState patrollingState = new PatrollingState();
    public readonly ChasingState chasingState = new ChasingState();
    public readonly AttackingState attackingState = new AttackingState();

    public void ChangeState(IEnemyState newState)
    {
        currentState = newState;
        currentStateName = newState.GetType().Name;   // e.g. "ChasingState"
        currentState.Enter(this);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;

        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        GameObject patrolPointParent = GameObject.FindWithTag("PatrolPoint");
        patrolPoints = patrolPointParent.GetComponentsInChildren<Transform>()
                       .Where(t => t != patrolPointParent.transform).ToArray();

        ChangeState(idleState);   // starting state
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            health -= 10;

            if (health <= 0)
                Die();
            else
                StartCoroutine(Blink());
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    IEnumerator Blink()
    {
        rend.material = hitMat;
        yield return new WaitForSeconds(0.1f);
        rend.material = originalMaterial;
    }

    private void Update()
    {
        LookForPlayer();

        currentState.Tick(this);   // delegation: the heart of the pattern

        rb.linearVelocity = Vector3.zero;

        LookAtPlayer();
        SetLastKnownPlayerPosition();
    }

    private void LookForPlayer()
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, maxVisionDistance))
        {
            canSeePlayer = hit.transform == playerTransform;

            // global trigger: spotting the player is handled by the Context
            if (canSeePlayer && currentState != attackingState && currentState != chasingState)
            {
                ChangeState(chasingState);
            }
        }
    }

    private void LookAtPlayer()
    {
        if (canSeePlayer)
        {
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
        }
    }

    private void SetLastKnownPlayerPosition()
    {
        if (canSeePlayer)
        {
            lastKnownPlayerPosition = playerTransform.position;
        }
    }

    public void Shoot()   // public so AttackingState can call it
    {
        if (Time.time > lastShotTime + fireRate)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.Normalize();

            Quaternion bulletRotation = Quaternion.LookRotation(directionToPlayer);

            float maxInaccuracy = 10f;
            float currentInaccuracy = bloom * maxInaccuracy;
            float randomJaw = Random.Range(-currentInaccuracy, currentInaccuracy);
            float randomPitch = Random.Range(-currentInaccuracy, currentInaccuracy);

            bulletRotation *= Quaternion.Euler(randomPitch, randomJaw + 90, 0f);

            AudioManager.instance.PlaySFX(shootingSFX, 0.5f);

            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletRotation);
            Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            lastShotTime = Time.time;
        }
    }
}