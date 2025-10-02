using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public BoxCollider2D attackHitbox;
    public int attackDamage = 20;

    private void Start()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }

    // 애니메이션 이벤트로 호출
    public void EnableHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
        }
    }

    // 애니메이션 이벤트로 호출
    public void DisableHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}