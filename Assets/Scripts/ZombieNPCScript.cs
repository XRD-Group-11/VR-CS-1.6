using UnityEngine;

public class ZombieNPCScript : MonoBehaviour, IDamageable
{
    public int health = 100; // Zombie's health
    public float speed = 2f; // Walking speed
    public Transform player; // Reference to the player
    public float attackRange = 1.5f; // Distance to attack
    public int attackDamage = 10; // Damage dealt to the player per attack
    public float attackCooldown = 2f; // Time between attacks

    private Animator animator;
    private bool isDead = false;
    private float lastAttackTime = 0f; // Track time since the last attack
    public GameObject spawner;
    private ZombieSpawner spawnerScript;
    
    

    void Start()
    {
        animator = GetComponent<Animator>();


        // Find the player by tag (assuming the player is tagged "Player")
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
        AudioManager.instance.Play("Zombe_Walking");
        FindSpawner();

    }

    void Update()
    {
        if (isDead || player == null) return;

        // Move towards the player
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            // Walk towards the player
            animator.SetBool("IsWalking", true);
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            transform.LookAt(player.position);
        }
        else
        {
            // Attack the player
            animator.SetBool("IsWalking", false);
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        // Check if the attack cooldown has elapsed
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");

            // Placeholder for player health logic
            IDamageable playerHealth = player.GetComponent<IDamageable>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
            else
            {
                Debug.LogWarning("PlayerHealth component not found on the player.");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

       health-=damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if(spawnerScript!=null)
            spawnerScript.KillZombie();
        isDead = true;
        animator.SetTrigger("Die");
        AudioManager.instance.Play("Zombe_Die");
        Destroy(gameObject, 3f); // Destroy the object after the animation plays
    }

    private void FindSpawner()
    {
        float detectionRadius = 5f; // Adjust radius as needed

        // Look for colliders within a detection radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Spawner"))
            {
                spawner = collider.gameObject;
                spawnerScript = spawner.GetComponent<ZombieSpawner>();
                Debug.Log("Spawner found: " + spawner.name);
                break;
            }
        }

        if (spawner == null)
        {
            Debug.LogWarning("No spawner found nearby!");
        }
    }
}
