using UnityEngine;
using UnityEngine.UI;

public class ZombieHealth : MonoBehaviour, IDamageable
{
    [SerializeField] public int currentHealth {get; set; }
    [SerializeField] private int maxHealth = 100;
    

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;  
    }

}