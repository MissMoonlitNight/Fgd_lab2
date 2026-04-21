using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    public float health = 50f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"[{gameObject.name}] Получено {amount} урона. Осталось HP: {health}");
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}