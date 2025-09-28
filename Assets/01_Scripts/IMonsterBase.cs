using UnityEngine;

public class MonsterBase : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public float attackDamage;
    public float attackRange;
    public float attackDelay;
    public bool isDead = false;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // 몬스터 사망 처리 로직 추가 요망
        Destroy(gameObject);
    }
}

