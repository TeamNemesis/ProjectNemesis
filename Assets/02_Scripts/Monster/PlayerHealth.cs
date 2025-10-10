using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageAble
{
    public int maxHealth;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= (int)damage; 
        Debug.Log("Player Health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
