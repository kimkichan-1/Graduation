using System.Collections;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float minMoveDuration = 1f;
    [SerializeField] private float maxMoveDuration = 3f;
    [SerializeField] private float minPauseDuration = 0.5f;
    [SerializeField] private float maxPauseDuration = 2f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private BoxCollider2D attackHitbox;

    [Header("Checks")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private float wallCheckDistance = 0.1f;
    [SerializeField] private float ledgeCheckDistance = 0.2f;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip walkSound;

    private Rigidbody2D rb;
    private MonsterHealth monsterHealth;
    private Animator animator;
    private AudioSource audioSource;
    private Renderer rend;

    private Vector2 startPosition;
    private bool isMovingRight = true;
    private bool isMoving = true;
    private Vector3 initialScale;
    private bool isChasingPlayer = false;
    private bool canAttack = true;
    private bool isAttacking = false;
    private bool isStaggered = false;
    private Transform player;
    private Coroutine attackCoroutine = null; // 공격 코루틴을 저장할 변수

    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int attackHash = Animator.StringToHash("Attack");

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        monsterHealth = GetComponent<MonsterHealth>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rend = GetComponent<Renderer>();

        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        startPosition = transform.position;
        initialScale = transform.localScale;

        if (attackHitbox != null) attackHitbox.enabled = false;

        if (monsterHealth != null && animator != null) StartCoroutine(MovementRoutine());
    }

    void Update()
    {
        if (monsterHealth != null && monsterHealth.IsDead || isStaggered) return;

        DetectPlayer();

        if (isChasingPlayer)
        {
            ChasePlayer();
        }
    }

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (playerCollider != null && playerCollider.CompareTag("Player"))
        {
            player = playerCollider.transform;
            isChasingPlayer = true;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRadius && canAttack && !isAttacking)
            {
                // 공격 코루틴을 변수에 저장
                attackCoroutine = StartCoroutine(AttackPlayer());
            }
        }
        else
        {
            isChasingPlayer = false;
            player = null;
        }
    }

    private void ChasePlayer()
    {
        if (player == null || isAttacking) return;

        if (IsLedgeOrWallAhead())
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetFloat(speedHash, 0f);
            isChasingPlayer = false;
            player = null;
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        float moveDir = Mathf.Sign(direction.x);

        float xScale = Mathf.Abs(initialScale.x);
        transform.localScale = new Vector3(xScale * moveDir, initialScale.y, initialScale.z);

        rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);

        isMovingRight = moveDir > 0;
        isMoving = true;

        animator.SetFloat(speedHash, moveSpeed);
    }

    private IEnumerator AttackPlayer()
    {
        canAttack = false;
        isAttacking = true;
        isMoving = false;
        rb.velocity = Vector2.zero;

        if (animator != null) animator.SetTrigger(attackHash);

        yield return new WaitForSeconds(1f); // 공격 애니메이션 시간

        yield return new WaitForSeconds(0.3f); // 공격 후 경직

        isAttacking = false;
        isMoving = true;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        attackCoroutine = null; // 코루틴이 끝나면 참조를 null로 초기화
    }

    private void UpdateSpriteDirection()
    {
        if (animator == null || isAttacking) return;

        float moveDir = isMovingRight ? 1f : -1f;
        float xScale = Mathf.Abs(initialScale.x);
        transform.localScale = new Vector3(xScale * moveDir, initialScale.y, initialScale.z);

        if (!isChasingPlayer && isMoving)
        {
            rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);
        }

        float currentSpeed = isMoving ? moveSpeed : 0f;
        animator.SetFloat(speedHash, currentSpeed);
    }

    private IEnumerator MovementRoutine()
    {
        while (true)
        {
            if (isChasingPlayer || isAttacking || isStaggered)
            {
                yield return null;
                continue;
            }

            if (IsLedgeOrWallAhead())
            {
                isMovingRight = !isMovingRight;
            }
            else
            {
                isMovingRight = Random.value > 0.5f;
            }

            isMoving = true;
            UpdateSpriteDirection();
            float moveDuration = Random.Range(minMoveDuration, maxMoveDuration);
            float moveTimer = 0f;

            while (moveTimer < moveDuration)
            {
                if (IsLedgeOrWallAhead())
                {
                    break;
                }

                moveTimer += Time.deltaTime;
                yield return null;
            }

            isMoving = false;
            rb.velocity = Vector2.zero;
            animator.SetFloat(speedHash, 0f);
            float pauseDuration = Random.Range(minPauseDuration, maxPauseDuration);
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    private bool IsLedgeOrWallAhead()
    {
        if (wallCheck == null || ledgeCheck == null) return false;

        float directionX = Mathf.Sign(transform.localScale.x);
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, new Vector2(directionX, 0), wallCheckDistance, groundLayer);

        RaycastHit2D groundHit = Physics2D.Raycast(ledgeCheck.position, Vector2.down, ledgeCheckDistance, groundLayer);

        return wallHit.collider != null || groundHit.collider == null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && attackHitbox != null && attackHitbox.enabled)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(attackDamage);
        }
    }

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

    public void PlayWalkSound()
    {
        if (rend != null && !rend.isVisible) return;
        if (walkSound != null && audioSource != null) audioSource.PlayOneShot(walkSound);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        if (wallCheck != null)
        {
            float directionX = Mathf.Sign(transform.localScale.x);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + new Vector3(directionX, 0, 0) * wallCheckDistance);
        }

        if (ledgeCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + Vector3.down * ledgeCheckDistance);
        }
    }

    public bool IsMovingRight => isMovingRight;
    public bool IsMoving => isMoving;
    public bool IsChasingPlayer => isChasingPlayer;

    public void ApplyKnockback(Vector2 direction, float force, float staggerDuration)
    {
        if (monsterHealth != null && monsterHealth.IsDead || isStaggered) return;

        // 진행 중인 공격이 있었다면 중단
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            isAttacking = false;
            canAttack = true; // 공격을 즉시 다시 할 수 있게 할지, 아니면 쿨다운을 적용할지 결정 필요
        }

        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // Hurt 애니메이션 재생
        if (animator != null) animator.SetTrigger("isKnockedBack");

        StartCoroutine(Stagger(staggerDuration));
    }

    private IEnumerator Stagger(float duration)
    {
        isStaggered = true;
        yield return new WaitForSeconds(duration);
        isStaggered = false;
    }
}
