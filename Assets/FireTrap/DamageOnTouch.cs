using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public int damageAmount = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 닿은 대상의 태그가 "Player"인지 확인
        if (other.CompareTag("Player"))
        {
            // Player 오브젝트에서 PlayerHealth 스크립트를 찾아서 TakeDamage 함수를 호출
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}
