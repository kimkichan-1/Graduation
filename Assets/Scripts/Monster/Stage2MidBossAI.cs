using System.Collections;
using UnityEngine;

/// <summary>
/// Stage2 중간보스 AI 컨트롤러
/// 일반 공격(Attack)과 특수 패턴(순간이동 Teleport)을 사용합니다.
/// </summary>
public class Stage2MidBossAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f; // 이동 속도
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
    [SerializeField] private int attackDamage = 20; // 근접 공격 데미지
    [SerializeField] private float attackCooldown = 2f; // 근접 공격 쿨타임
    [SerializeField] private BoxCollider2D attackHitbox; // 근접 공격 히트박스

    [Header("Teleport Settings")]
    [SerializeField] private float teleportMinX = -10f; // 순간이동 최소 X 위치
    [SerializeField] private float teleportMaxX = 10f; // 순간이동 최대 X 위치
    [SerializeField] private float teleportYOffset = 0f; // Y 위치 오프셋 (바닥 기준)
    [SerializeField] private float teleportCooldown = 5f; // 순간이동 쿨타임
    [SerializeField] private float teleportRange = 8f; // 순간이동 사용 거리
    [SerializeField] private float teleportFadeDuration = 0.3f; // 페이드 인/아웃 시간
    [SerializeField] private GameObject teleportEffectPrefab; // 순간이동 이펙트 (옵션)

    [Header("Wall & Ledge Checks")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private float wallCheckDistance = 0.1f;
    [SerializeField] private float ledgeCheckDistance = 0.2f;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip teleportSound; // 순간이동 사운드
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
    private SpriteRenderer spriteRenderer; // 페이드 효과용
    private Renderer rend;

    // 상태 변수
    private bool isMovingRight = true;
    private bool isMoving = true;
    private Vector3 initialScale;
    private Vector3 initialSpriteLocalPosition;
    private bool isChasingPlayer = false;
    private bool canAttack = true;
    private bool isAttacking = false;
    private bool isTeleporting = false; // 순간이동 중인지 확인
    private bool canTeleport = true; // 순간이동 가능 여부
    private bool isStaggered = false;
    private Transform player;
    private Coroutine attackCoroutine = null;
    private bool isActivated = false; // ★★★ 보스 활성화 여부 ★★★

    // 애니메이터 해시
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int isAttackingHash = Animator.StringToHash("isAttacking");

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        monsterHealth = GetComponent<MonsterHealth>();
        audioSource = GetComponent<AudioSource>();

        // ★★★ 스프라이트 자식 구조일 때 Animator와 Renderer를 자식에서 찾기 ★★★
        if (useSpriteChild && spriteTransform != null)
        {
            animator = spriteTransform.GetComponent<Animator>();
            rend = spriteTransform.GetComponent<Renderer>();
            spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
            initialSpriteLocalPosition = spriteTransform.localPosition;

            Debug.Log($"[Stage2MidBoss] 스프라이트 자식 모드: Animator={animator != null}, Renderer={rend != null}");
        }
        else
        {
            animator = GetComponent<Animator>();
            rend = GetComponent<Renderer>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        initialScale = transform.localScale;

        if (attackHitbox != null) attackHitbox.enabled = false;

        if (monsterHealth != null && animator != null)
        {
            StartCoroutine(MovementRoutine());
            Debug.Log("[Stage2MidBoss] MovementRoutine 시작!");
        }
        else
        {
            Debug.LogError($"[Stage2MidBoss] 초기화 실패! MonsterHealth={monsterHealth != null}, Animator={animator != null}");
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

        // 공격, 경직, 순간이동 중에는 모든 동작 중지
        if (isAttacking || isStaggered || isTeleporting)
        {
            return; // 다른 동작 중지 (velocity는 FixedUpdate에서 처리)
        }

        DetectPlayer();

        if (isChasingPlayer)
        {
            ChasePlayer();
        }
    }

    void FixedUpdate()
    {
        // 공격, 경직, 순간이동 중에는 물리적으로 움직임 차단
        if (isAttacking || isStaggered || isTeleporting)
        {
            rb.linearVelocity = Vector2.zero;
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
                Debug.Log("[Stage2MidBoss] 플레이어 감지! 보스 활성화!");
            }

            player = playerCollider.transform;
            isChasingPlayer = true;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // 순간이동 범위 내에 있고, 쿨타임이 끝났으면 순간이동 시도 (Idle/Run 상태일 때만)
            if (distanceToPlayer <= teleportRange && canTeleport && !isTeleporting && !isAttacking && !isStaggered)
            {
                // Animator 상태 확인: Idle 또는 Run 상태일 때만 순간이동
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                bool isIdleOrRun = !stateInfo.IsName("Attack1") && !stateInfo.IsName("Attack2") && !stateInfo.IsName("TakeHit") && !stateInfo.IsName("Death");

                if (isIdleOrRun && Random.value < 0.3f)
                {
                    StartCoroutine(PerformTeleport());
                }
            }
            // 근접 공격 범위 내에 있으면 일반 공격
            else if (canAttack && !isAttacking && !isTeleporting)
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
        if (player == null || isAttacking || isTeleporting) return;

        // 이동 범위 제한 체크 (텔레포트 범위와 동일)
        if (IsOutOfBounds())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetFloat(speedHash, 0f);
            isChasingPlayer = false;
            player = null;
            return;
        }

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

        // 이동 예정 위치 계산
        float nextX = transform.position.x + (moveDir * moveSpeed * Time.deltaTime);

        // 이동 후 범위를 벗어나는지 체크
        if (nextX < teleportMinX || nextX > teleportMaxX)
        {
            // 범위 밖으로 나가려고 하면 이동 중단
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetFloat(speedHash, 0f);
            isChasingPlayer = false;
            player = null;
            return;
        }

        // 스프라이트 뒤집기
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

        if (animator != null)
        {
            animator.SetBool(isAttackingHash, true);
            animator.SetFloat(speedHash, 0f); // Speed 값도 0으로 설정
            Debug.Log("[Stage2MidBoss] 공격 시작 - isAttacking=true, Speed=0");
        }

        // Attack1 애니메이션 상태로 전환될 때까지 대기 (최대 0.5초)
        float waitTime = 0f;
        float maxWaitTime = 0.5f;

        while (waitTime < maxWaitTime)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack1"))
            {
                // Attack1 상태 진입 성공!
                Debug.Log("[Stage2MidBoss] Attack1 상태 진입 성공!");
                break;
            }
            yield return new WaitForSeconds(0.05f);
            waitTime += 0.05f;
        }

        // Attack1 애니메이션이 완전히 끝날 때까지 대기 (normalizedTime으로 확인)
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack1"))
            {
                // 애니메이션 길이 가져오기
                float attackAnimLength = stateInfo.length;
                Debug.Log($"[Stage2MidBoss] Attack1 애니메이션 재생 시작 (길이: {attackAnimLength}초)");

                // normalizedTime이 0.95 이상이 될 때까지 대기 (애니메이션이 거의 끝날 때까지)
                float safetyTimer = 0f;
                float maxSafetyTime = attackAnimLength + 1f; // 안전 타이머

                while (safetyTimer < maxSafetyTime)
                {
                    AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

                    // 현재 상태가 Attack1이고, normalizedTime이 0.95 이상이면 종료
                    if (currentState.IsName("Attack1") && currentState.normalizedTime >= 0.95f)
                    {
                        Debug.Log($"[Stage2MidBoss] Attack1 애니메이션 완료 (normalizedTime: {currentState.normalizedTime})");
                        break;
                    }

                    // 상태가 바뀌었다면 (자동 전환된 경우)
                    if (!currentState.IsName("Attack1"))
                    {
                        Debug.Log($"[Stage2MidBoss] Attack1에서 다른 상태로 자동 전환됨");
                        break;
                    }

                    yield return null;
                    safetyTimer += Time.deltaTime;
                }

                Debug.Log($"[Stage2MidBoss] Attack1 대기 완료 (경과 시간: {safetyTimer}초)");
            }
            else
            {
                // Attack1 상태 진입 실패 (기본값으로 대기)
                Debug.LogWarning($"[Stage2MidBoss] Attack1 상태 진입 실패!");
                yield return new WaitForSeconds(1.0f);
            }
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
        }

        // 공격 애니메이션 종료 처리
        if (animator != null)
        {
            animator.SetBool(isAttackingHash, false);
            animator.SetFloat(speedHash, 0f);
            Debug.Log("[Stage2MidBoss] 공격 애니메이션 종료 - isAttacking=false");
        }

        yield return new WaitForSeconds(0.1f);

        // 상태 초기화
        isAttacking = false;
        isMoving = false; // 일단 정지 상태로
        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            // 현재 상태 확인
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log($"[Stage2MidBoss] 공격 종료 전 상태: {currentState.IsName("Attack1")}, normalizedTime: {currentState.normalizedTime}");

            // Idle 애니메이션으로 강제 전환 (여러 가능한 이름 시도)
            if (animator.HasState(0, Animator.StringToHash("Idle")))
            {
                animator.Play("Idle", 0, 0f);
                Debug.Log("[Stage2MidBoss] Idle 상태로 전환");
            }
            else if (animator.HasState(0, Animator.StringToHash("Idle1")))
            {
                animator.Play("Idle1", 0, 0f);
                Debug.Log("[Stage2MidBoss] Idle1 상태로 전환");
            }
            else
            {
                Debug.LogWarning("[Stage2MidBoss] Idle 상태를 찾을 수 없습니다!");
            }
        }

        yield return new WaitForSeconds(0.3f);

        // 다시 확인
        if (animator != null)
        {
            AnimatorStateInfo afterState = animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log($"[Stage2MidBoss] 공격 종료 후 상태: Idle={afterState.IsName("Idle")}, Idle1={afterState.IsName("Idle1")}");
        }

        // 이제 다시 움직일 수 있음
        isMoving = true;
        Debug.Log("[Stage2MidBoss] isMoving = true, 일반 행동 재개");

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        attackCoroutine = null;
    }

    /// <summary>
    /// 순간이동 패턴 (지정된 X 범위 내 랜덤 위치로 이동)
    /// </summary>
    private IEnumerator PerformTeleport()
    {
        canTeleport = false;
        isTeleporting = true;
        isMoving = false;
        rb.linearVelocity = Vector2.zero;

        // 순간이동 사운드
        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }

        // 페이드 아웃 (투명해지기)
        yield return StartCoroutine(FadeOut(teleportFadeDuration));

        // 순간이동 이펙트 (사라지는 곳)
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
        }

        // 랜덤 X 위치 계산 (지정된 범위 내)
        float randomX = Random.Range(teleportMinX, teleportMaxX);

        // Y 위치는 현재 Y + 오프셋 (또는 바닥 감지)
        float newY = transform.position.y + teleportYOffset;

        // 새 위치로 순간이동
        Vector3 newPosition = new Vector3(randomX, newY, transform.position.z);
        transform.position = newPosition;

        Debug.Log($"[Stage2MidBoss] 순간이동! 위치: {newPosition}");

        // 순간이동 이펙트 (나타나는 곳)
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
        }

        // 잠깐 대기
        yield return new WaitForSeconds(0.2f);

        // 페이드 인 (다시 나타나기)
        yield return StartCoroutine(FadeIn(teleportFadeDuration));

        // 순간이동 사운드 (다시)
        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }

        yield return new WaitForSeconds(0.3f);

        isTeleporting = false;
        isMoving = true;

        // 순간이동 쿨타임
        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
    }

    /// <summary>
    /// 페이드 아웃 (투명해지기)
    /// </summary>
    private IEnumerator FadeOut(float duration)
    {
        if (spriteRenderer == null) yield break;

        float elapsed = 0f;
        Color color = spriteRenderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);
    }

    /// <summary>
    /// 페이드 인 (다시 나타나기)
    /// </summary>
    private IEnumerator FadeIn(float duration)
    {
        if (spriteRenderer == null) yield break;

        float elapsed = 0f;
        Color color = spriteRenderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        spriteRenderer.color = new Color(color.r, color.g, color.b, 1f);
    }

    private void UpdateSpriteDirection()
    {
        if (animator == null || isAttacking || isTeleporting) return;

        float moveDir = isMovingRight ? 1f : -1f;

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
            // 스프라이트 자식 오브젝트 사용 (방향 수정: 오른쪽=양수, 왼쪽=음수)
            Vector3 spriteScale = spriteTransform.localScale;
            spriteScale.x = moveDir > 0 ? Mathf.Abs(spriteScale.x) : -Mathf.Abs(spriteScale.x);
            spriteTransform.localScale = spriteScale;

            // 로컬 위치 보정
            Vector3 correctedPosition = initialSpriteLocalPosition;
            correctedPosition.x *= (moveDir > 0 ? 1f : -1f);
            spriteTransform.localPosition = correctedPosition;
        }
        else
        {
            // 기본 방식
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

            if (isChasingPlayer || isAttacking || isTeleporting || isStaggered)
            {
                yield return null;
                continue;
            }

            // 범위를 벗어났거나 절벽/벽이 있으면 방향 전환
            if (IsOutOfBounds() || IsLedgeOrWallAhead())
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
                // 공격이나 다른 액션이 시작되면 즉시 중단
                if (isAttacking || isTeleporting || isStaggered || isChasingPlayer)
                {
                    break;
                }

                // 범위를 벗어나거나 절벽/벽이 있으면 중단
                if (IsOutOfBounds() || IsLedgeOrWallAhead())
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

    /// <summary>
    /// 이동 범위를 벗어났는지 체크 (텔레포트 범위와 동일)
    /// </summary>
    private bool IsOutOfBounds()
    {
        float currentX = transform.position.x;
        return currentX <= teleportMinX || currentX >= teleportMaxX;
    }

    private bool IsLedgeOrWallAhead()
    {
        if (wallCheck == null || ledgeCheck == null) return false;

        float directionX;
        if (useSpriteChild)
        {
            directionX = isMovingRight ? 1f : -1f;
        }
        else
        {
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

        // 순간이동 범위 (파란색)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, teleportRange);

        // 순간이동 가능 영역 (초록색 선)
        Gizmos.color = Color.green;
        float y = transform.position.y + teleportYOffset;
        Gizmos.DrawLine(new Vector3(teleportMinX, y, 0), new Vector3(teleportMaxX, y, 0));

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
    public bool IsTeleporting => isTeleporting;
}
