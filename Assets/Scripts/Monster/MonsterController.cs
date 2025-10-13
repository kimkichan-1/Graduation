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
    [SerializeField] private Vector2 detectionSize = new Vector2(10f, 10f); // 감지 범위 크기 (가로, 세로)
    [SerializeField] private Vector2 detectionOffset = Vector2.zero; // 감지 범위 오프셋
    [SerializeField] private Vector2 attackSize = new Vector2(2f, 2f); // 공격 범위 크기 (가로, 세로)
    [SerializeField] private Vector2 attackOffset = Vector2.zero; // 공격 범위 오프셋
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
    private readonly int isAttackingHash = Animator.StringToHash("isAttacking");

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
        // 죽었으면 모든 동작 중지
        if (monsterHealth != null && monsterHealth.IsDead)
        {
            rb.linearVelocity = Vector2.zero; // 이동 멈춤
            animator.SetFloat(speedHash, 0f); // 애니메이션 속도 0
            return;
        }

        if (isStaggered) return;

        DetectPlayer();

        if (isChasingPlayer)
        {
            ChasePlayer();
        }
    }

    private void DetectPlayer()
    {
        // 사각형 범위로 플레이어 감지 (오프셋 적용)
        Vector2 detectionCenter = (Vector2)transform.position + detectionOffset;
        Collider2D playerCollider = Physics2D.OverlapBox(detectionCenter, detectionSize, 0f, playerLayer);

        if (playerCollider != null && playerCollider.CompareTag("Player"))
        {
            player = playerCollider.transform;
            isChasingPlayer = true;

            // 사각형 범위로 공격 가능 여부 체크 (오프셋 적용)
            Vector2 attackCenter = (Vector2)transform.position + attackOffset;
            Collider2D attackCheck = Physics2D.OverlapBox(attackCenter, attackSize, 0f, playerLayer);

            if (attackCheck != null && canAttack && !isAttacking)
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
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetFloat(speedHash, 0f);
            isChasingPlayer = false;
            player = null;
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        float moveDir = Mathf.Sign(direction.x);

        float xScale = Mathf.Abs(initialScale.x);
        transform.localScale = new Vector3(xScale * moveDir, initialScale.y, initialScale.z);

        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);

        isMovingRight = moveDir > 0;
        isMoving = true;

        animator.SetFloat(speedHash, moveSpeed);
    }

    private IEnumerator AttackPlayer()
    {
        canAttack = false;
        isAttacking = true;
        isMoving = false;
        rb.linearVelocity = Vector2.zero;

        if (animator != null) animator.SetBool(isAttackingHash, true);

        yield return new WaitForSeconds(0.8f); // 공격 애니메이션 시간 (애니메이션 끝나기 직전)

        // 애니메이션이 끝나기 전에 Bool을 false로 전환하여 부드러운 전환 보장
        if (animator != null) animator.SetBool(isAttackingHash, false);

        yield return new WaitForSeconds(0.5f); // 공격 후 경직

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
            rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
        }

        float currentSpeed = isMoving ? moveSpeed : 0f;
        animator.SetFloat(speedHash, currentSpeed);
    }

    private IEnumerator MovementRoutine()
    {
        while (true)
        {
            // 죽었으면 움직임 코루틴 종료
            if (monsterHealth != null && monsterHealth.IsDead)
            {
                yield break;
            }

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
            rb.linearVelocity = Vector2.zero;
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
        // 빨간색 사각형 - 감지 범위 (오프셋 적용)
        Gizmos.color = Color.red;
        Vector3 detectionCenter = transform.position + (Vector3)detectionOffset;
        Gizmos.DrawWireCube(detectionCenter, new Vector3(detectionSize.x, detectionSize.y, 0));

        // 노란색 사각형 - 공격 범위 (오프셋 적용)
        Gizmos.color = Color.yellow;
        Vector3 attackCenter = transform.position + (Vector3)attackOffset;
        Gizmos.DrawWireCube(attackCenter, new Vector3(attackSize.x, attackSize.y, 0));

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

        rb.linearVelocity = Vector2.zero;
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
