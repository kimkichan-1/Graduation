using System.Collections;
using UnityEngine;

public class ClamMonsterController : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float attackRadius = 3f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private BoxCollider2D attackHitbox;

    private Rigidbody2D rb;
    private MonsterHealth monsterHealth;
    private Animator animator;
    private AudioSource audioSource;
    private Vector3 initialScale;

    private bool canAttack = true;
    private bool isAttacking = false;
    private bool isStaggered = false;
    private Transform player;
    private Coroutine attackCoroutine = null;

    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int hurtHash = Animator.StringToHash("isHurt");

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        monsterHealth = GetComponent<MonsterHealth>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        initialScale = transform.localScale;
        if (attackHitbox != null) attackHitbox.enabled = false;
    }

    void Update()
    {
        if (monsterHealth != null && monsterHealth.IsDead || isStaggered) return;

        DetectPlayer();
    }

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (playerCollider != null)
        {
            player = playerCollider.transform;
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRadius && canAttack && !isAttacking)
            {
                attackCoroutine = StartCoroutine(AttackPlayer());
            }
        }
        else
        {
            player = null;
        }
    }

    private IEnumerator AttackPlayer()
    {
        canAttack = false;
        isAttacking = true;

        if (player != null)
        {
            float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x) * directionToPlayer, initialScale.y, initialScale.z);
        }

        if (animator != null) animator.SetTrigger(attackHash);

        yield return new WaitForSeconds(1.5f); // 공격 애니메이션 길이에 맞춰 대기 (임시)

        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        attackCoroutine = null;
    }

    // 넉백/피격 관련 함수들
    public void ApplyKnockback(Vector2 direction, float force, float staggerDuration)
    {
        if (monsterHealth != null && monsterHealth.IsDead || isStaggered) return;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            isAttacking = false;
            canAttack = true;
        }

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        if (animator != null) animator.SetTrigger(hurtHash);

        StartCoroutine(Stagger(staggerDuration));
    }

    private IEnumerator Stagger(float duration)
    {
        isStaggered = true;
        yield return new WaitForSeconds(duration);
        isStaggered = false;
    }

    // --- 애니메이션 이벤트용 함수들 ---
    public void EnableAttackHitbox()
    {
        if (attackHitbox != null) attackHitbox.enabled = true;
    }

    public void DisableAttackHitbox()
    {
        if (attackHitbox != null) attackHitbox.enabled = false;
    }

    public void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null) audioSource.PlayOneShot(attackSound);
    }
}
