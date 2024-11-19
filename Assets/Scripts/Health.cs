using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] public int currentHealth {get; set; }
    [SerializeField] private int maxHealth = 100;
    
    [SerializeField] private Sprite[] digitSprites;  
    [SerializeField] private Image[] digitImages;    

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateNumericDisplay(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;  
        UpdateNumericDisplay(currentHealth);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateNumericDisplay(int health)
    {
        var hundreds = health / 100;
        var tens = (health % 100) / 10;
        var ones = health % 10;

        if (health < 100)
        {
            digitImages[0].gameObject.SetActive(false);  
        }
        else
        {
            digitImages[0].gameObject.SetActive(true);   
            digitImages[0].sprite = digitSprites[hundreds];
        }

      
        if (health < 10)
        {
            digitImages[1].gameObject.SetActive(false);  
        }
        else
        {
            digitImages[1].gameObject.SetActive(true);   
            digitImages[1].sprite = digitSprites[tens];
        }
        
        digitImages[2].sprite = digitSprites[ones];
    }
}