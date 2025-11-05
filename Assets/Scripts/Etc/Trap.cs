using UnityEngine;

public class Trap : MonoBehaviour
{
    public int damageAmount = 10; // 입힐 데미지

    private void OnTriggerEnter2D(Collider2D other) // 2D용
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}
