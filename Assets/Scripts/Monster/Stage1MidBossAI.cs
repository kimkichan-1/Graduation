using System.Collections;
using UnityEngine;

/// <summary>
/// Stage1 중간보스 AI 컨트롤러
/// 일반 공격(Attack)과 특수 공격(Cast -> Spell 소환)을 사용합니다.
/// </summary>
public class Stage1MidBossAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1.5f; // 이동 속도
    [SerializeField] private float minMoveDuration = 2f;
    [SerializeField] private float maxMoveDuration = 4f;
    [SerializeField] private float minPauseDuration = 1f;
    [SerializeField] private float maxPauseDuration = 3f;

    [Header("Detection Settings")]
    [SerializeField] private Vector2 detectionSize = new Vector2(12f, 10f); // 감지 범위
    [SerializeField] private Vector2 detectionOffset = Vector2.zero;
    [SerializeField] private Vector2 attackSize = new Vector2(3f, 3f); // 근접 공격 범위
    [SerializeField] private Vector2 attackOffset = Vector2.zero;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 15; // 근접 공격 데미지
    [SerializeField] private float attackCooldown = 2f; // 근접 공격 쿨타임
    [SerializeField] private BoxCollider2D attackHitbox; // 근접 공격 히트박스

    [Header("Special Attack (Cast/Spell) Settings")]
    [SerializeField] private GameObject spellPrefab; // Spell 이펙트 프리팹
    [SerializeField] private float castRange = 8f; // Cast 공격 사용 거리
    [SerializeField] private float castCooldown = 5f; // Cast 공격 쿨타임
    [SerializeField] private float castAnimationDuration = 1f; // Cast 애니메이션 길이
    [SerializeField] private Vector3 spellSpawnOffset = new Vector3(0, 3f, 0); // Spell 소환 위치 (플레이어 위)

    [Header("Wall & Ledge Checks")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private float wallCheckDistance = 0.1f;
    [SerializeField] private float ledgeCheckDistance = 0.2f;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip castSound; // Cast 시전 사운드
    [SerializeField] private AudioClip walkSound;

    [Header("Sprite Settings (피벗 문제 해결용)")]
    [Tooltip("스프라이트가 중앙에 없을 때 체크하세요")]
    [SerializeField] private bool useSpriteChild = false;
    [Tooltip("스프라이트 자식 오브젝트 (선택사항)")]
    [SerializeField] private Transform spriteTransform;

    // 컴포넌트 참조
    private Rigidbody2D rb;
    private MonsterHealth monsterHealth;
    private Animator animator;
    private AudioSource audioSource;
    private Renderer rend;

    // 상태 변수
    private bool isMovingRight = true;
    private bool isMoving = true;
    private Vector3 initialScale;
    private Vector3 initialSpriteLocalPosition; // 스프라이트 자식의 초기 위치 저장
    private bool isChasingPlayer = false;
    private bool canAttack = true;
    private bool isAttacking = false;
    private bool isCasting = false; // Cast 중인지 확인
    private bool canCast = true; // Cast 가능 여부
    private bool isStaggered = false;
    private Transform player;
    private Coroutine attackCoroutine = null;
    private bool isActivated = false; // ★★★ 보스 활성화 여부 ★★★

    // 애니메이터 해시
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int isAttackingHash = Animator.StringToHash("isAttacking");
    private readonly int isCastingHash = Animator.StringToHash("isCasting");

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        monsterHealth = GetComponent<MonsterHealth>();
        audioSource = GetComponent<AudioSource>();

        // ★★★ 스프라이트 자식 구조일 때 Animator와 Renderer를 자식에서 찾기 ★★★
        if (useSpriteChild && spriteTransform != null)
        {
            // 자식의 Animator와 Renderer 가져오기
            animator = spriteTransform.GetComponent<Animator>();
            rend = spriteTransform.GetComponent<Renderer>();
            initialSpriteLocalPosition = spriteTransform.localPosition;

            Debug.Log($"[MidBoss] 스프라이트 자식 모드: Animator={animator != null}, Renderer={rend != null}");
        }
        else
        {
            // 기본 방식: 부모에서 가져오기
            animator = GetComponent<Animator>();
            rend = GetComponent<Renderer>();
        }

        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        initialScale = transform.localScale;

        if (attackHitbox != null) attackHitbox.enabled = false;

        if (monsterHealth != null && animator != null)
        {
            StartCoroutine(MovementRoutine());
            Debug.Log("[MidBoss] MovementRoutine 시작!");
        }
        else
        {
            Debug.LogError($"[MidBoss] 초기화 실패! MonsterHealth={monsterHealth != null}, Animator={animator != null}");
        }
    }

    void Update()
    {
        // 죽었으면 모든 동작 중지
        if (monsterHealth != null && monsterHealth.IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat(speedHash, 0f);
            return;
        }

        if (isStaggered || isCasting) return; // Cast 중에는 다른 동작 불가

        DetectPlayer();

        if (isChasingPlayer)
        {
            ChasePlayer();
        }
    }

    private void DetectPlayer()
    {
        // 플레이어 감지
        Vector2 detectionCenter = (Vector2)transform.position + detectionOffset;
        Collider2D playerCollider = Physics2D.OverlapBox(detectionCenter, detectionSize, 0f, playerLayer);

        if (playerCollider != null && playerCollider.CompareTag("Player"))
        {
            // ★★★ 첫 감지 시 보스 활성화 ★★★
            if (!isActivated)
            {
                isActivated = true;
                Debug.Log("[MidBoss] 플레이어 감지! 보스 활성화!");
            }

            player = playerCollider.transform;
            isChasingPlayer = true;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Cast 공격 범위 내에 있고, 쿨타임이 끝났으면 Cast 공격 시도
            if (distanceToPlayer <= castRange && canCast && !isCasting && !isAttacking)
            {
                StartCoroutine(PerformCastAttack());
            }
            // 근접 공격 범위 내에 있으면 일반 공격
            else if (canAttack && !isAttacking && !isCasting)
            {
                Vector2 attackCenter = (Vector2)transform.position + attackOffset;
                Collider2D attackCheck = Physics2D.OverlapBox(attackCenter, attackSize, 0f, playerLayer);

                if (attackCheck != null)
                {
                    attackCoroutine = StartCoroutine(AttackPlayer());
                }
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
        if (player == null || isAttacking || isCasting) return;

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

        // 스프라이트 뒤집기 (피벗 문제 해결 포함)
        FlipSprite(moveDir);

        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);

        isMovingRight = moveDir > 0;
        isMoving = true;

        animator.SetFloat(speedHash, moveSpeed);
    }

    // 근접 공격
    private IEnumerator AttackPlayer()
    {
        canAttack = false;
        isAttacking = true;
        isMoving = false;
        rb.linearVelocity = Vector2.zero;

        if (animator != null) animator.SetBool(isAttackingHash, true);

        yield return new WaitForSeconds(0.8f);

        if (animator != null) animator.SetBool(isAttackingHash, false);

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        isMoving = true;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        attackCoroutine = null;
    }

    // Cast 공격 (Spell 소환)
    private IEnumerator PerformCastAttack()
    {
        canCast = false;
        isCasting = true;
        isMoving = false;
        rb.linearVelocity = Vector2.zero;

        // Cast 애니메이션 시작
        if (animator != null) animator.SetBool(isCastingHash, true);

        // Cast 사운드 재생
        if (castSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(castSound);
        }

        // Cast 애니메이션 지속 시간 대기
        yield return new WaitForSeconds(castAnimationDuration);

        // Spell 소환 (플레이어 위치 기준)
        if (spellPrefab != null && player != null)
        {
            Vector3 spawnPosition = player.position + spellSpawnOffset;
            GameObject spellInstance = Instantiate(spellPrefab, spawnPosition, Quaternion.identity);

            // Spell 스크립트에 데미지 정보 전달 (옵션)
            SpellAttack spellAttack = spellInstance.GetComponent<SpellAttack>();
            if (spellAttack != null)
            {
                spellAttack.Initialize(attackDamage);
            }
        }

        // Cast 애니메이션 종료
        if (animator != null) animator.SetBool(isCastingHash, false);

        yield return new WaitForSeconds(0.3f);

        isCasting = false;
        isMoving = true;

        // Cast 쿨타임
        yield return new WaitForSeconds(castCooldown);
        canCast = true;
    }

    private void UpdateSpriteDirection()
    {
        if (animator == null || isAttacking || isCasting) return;

        float moveDir = isMovingRight ? 1f : -1f;

        // 스프라이트 뒤집기 (피벗 문제 해결 포함)
        FlipSprite(moveDir);

        if (!isChasingPlayer && isMoving)
        {
            rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
        }

        float currentSpeed = isMoving ? moveSpeed : 0f;
        animator.SetFloat(speedHash, currentSpeed);
    }

    /// <summary>
    /// 스프라이트 방향 전환 (피벗 문제 해결)
    /// </summary>
    private void FlipSprite(float moveDir)
    {
        if (useSpriteChild && spriteTransform != null)
        {
            // 방법 2: 스프라이트 자식 오브젝트 사용
            // 부모는 회전하지 않고, 자식 스프라이트만 뒤집기
            // ★★★ 스프라이트가 반대 방향을 보고 있다면 moveDir의 부호를 반대로 ★★★
            Vector3 spriteScale = spriteTransform.localScale;
            spriteScale.x = moveDir > 0 ? -Mathf.Abs(spriteScale.x) : Mathf.Abs(spriteScale.x); // 부호 반대!
            spriteTransform.localScale = spriteScale;

            // 로컬 위치도 보정 (스프라이트가 오른쪽에 치우쳐 있는 경우)
            Vector3 correctedPosition = initialSpriteLocalPosition;
            correctedPosition.x *= (moveDir > 0 ? -1f : 1f); // 부호 반대!
            spriteTransform.localPosition = correctedPosition;
        }
        else
        {
            // 방법 1: 기본 방식 (피벗이 중앙인 경우)
            float xScale = Mathf.Abs(initialScale.x);
            transform.localScale = new Vector3(xScale * moveDir, initialScale.y, initialScale.z);
        }
    }

    private IEnumerator MovementRoutine()
    {
        while (true)
        {
            if (monsterHealth != null && monsterHealth.IsDead)
            {
                yield break;
            }

            // ★★★ 보스가 활성화되지 않았으면 대기 ★★★
            if (!isActivated)
            {
                yield return null;
                continue;
            }

            if (isChasingPlayer || isAttacking || isCasting || isStaggered)
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

        // ★★★ 스프라이트 자식 구조일 때는 isMovingRight 변수 사용 ★★★
        float directionX;
        if (useSpriteChild)
        {
            // 스프라이트 자식 모드: isMovingRight 변수로 방향 판단
            directionX = isMovingRight ? 1f : -1f;
        }
        else
        {
            // 기본 모드: transform.localScale.x로 방향 판단
            directionX = Mathf.Sign(transform.localScale.x);
        }

        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, new Vector2(directionX, 0), wallCheckDistance, groundLayer);
        RaycastHit2D groundHit = Physics2D.Raycast(ledgeCheck.position, Vector2.down, ledgeCheckDistance, groundLayer);

        return wallHit.collider != null || groundHit.collider == null;
    }

    // 근접 공격 히트박스 트리거
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && attackHitbox != null && attackHitbox.enabled)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(attackDamage);
        }
    }

    // 애니메이션 이벤트용 함수들
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

    // 넉백 처리
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

        if (animator != null) animator.SetTrigger("isKnockedBack");

        StartCoroutine(Stagger(staggerDuration));
    }

    private IEnumerator Stagger(float duration)
    {
        isStaggered = true;
        yield return new WaitForSeconds(duration);
        isStaggered = false;
    }

    // 디버그용 기즈모
    private void OnDrawGizmos()
    {
        // 감지 범위 (빨간색)
        Gizmos.color = Color.red;
        Vector3 detectionCenter = transform.position + (Vector3)detectionOffset;
        Gizmos.DrawWireCube(detectionCenter, new Vector3(detectionSize.x, detectionSize.y, 0));

        // 근접 공격 범위 (노란색)
        Gizmos.color = Color.yellow;
        Vector3 attackCenter = transform.position + (Vector3)attackOffset;
        Gizmos.DrawWireCube(attackCenter, new Vector3(attackSize.x, attackSize.y, 0));

        // Cast 공격 범위 (파란색)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, castRange);

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

    // 외부에서 상태 확인용 프로퍼티
    public bool IsMovingRight => isMovingRight;
    public bool IsMoving => isMoving;
    public bool IsChasingPlayer => isChasingPlayer;
}
