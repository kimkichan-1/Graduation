using UnityEngine;
using System.Collections;

// 플레이어의 이동, 점프, 대시, 벽 슬라이드, 적과의 상호작용 등을 제어하는 스크립트
public class PlayerController : MonoBehaviour
{
    // ========================== UI 참조 ==========================
    public StatsUIManager statsUIManager; // 스탯 UI 관리자

    // ========================== 설정 값 ==========================
    [Header("운동 설정")] // Unity Inspector에서 '운동 설정' 섹션을 그룹화
    // 점프 시 적용되는 힘
    public float jumpForce = 300f;
    // 벽 점프 시 적용되는 힘
    public float wallJumpForce = 300f;
    // 기본 이동 속도
    public float moveSpeed = 5f;
    // 공격 중 이동 속도 (감소된 속도)
    public float attackMoveSpeed = 1f;
    // 대시 시 적용되는 힘
    public float dashForce = 10f;
    // 대시 지속 시간
    public float dashDuration = 0.2f;
    // 대시 쿨다운 시간
    public float dashCooldown = 1f;

    [Header("적 트리거 설정")] // Unity Inspector에서 '적 트리거 설정' 섹션을 그룹화
    // 적과 충돌 시 이동 속도 감소 비율
    public float speedReductionFactor = 0.5f;
    // 속도 감소 지속 시간
    public float speedReductionDuration = 0.5f;

    [Header("이펙트 설정")] // Unity Inspector에서 '이펙트 설정' 섹션을 그룹화
    // 점프 시 생성되는 먼지 효과 프리팹
    public GameObject jumpDustPrefab;
    // 먼지 효과 생성 위치
    public Transform dustSpawnPoint;
    // 대시 시 생성되는 효과 프리팹
    public GameObject dashEffectPrefab;

    [Header("벽 슬라이드 설정")] // Unity Inspector에서 '벽 슬라이드 설정' 섹션을 그룹화
    // 벽 슬라이드 시 하강 속도
    public float wallSlideSpeed = 1f;
    // 벽 감지 위치
    public Transform wallCheck;
    // 벽 감지 원의 반지름
    public float wallCheckRadius = 0.1f;
    // 벽으로 간주되는 레이어
    public LayerMask wallLayer;

    [Header("사운드 설정")] // Unity Inspector에서 '사운드 설정' 섹션을 그룹화
    // 점프 사운드
    public AudioClip jumpSound;
    // 걷기 사운드
    public AudioClip walkSound;
    // 기본 대시 사운드
    public AudioClip defaultDashSound;
    // 검 장착 시 대시 사운드
    public AudioClip swordDashSound;
    // 철퇴 장착 시 대시 사운드
    public AudioClip maceDashSound;
    // 창 장착 시 대시 사운드
    public AudioClip lanceDashSound;
    // 검 장착 사운드
    public AudioClip swordEquipSound;
    // 창 장착 사운드
    public AudioClip lanceEquipSound;
    // 철퇴 장착 사운드
    public AudioClip maceEquipSound;
    // 착지 사운드
    public AudioClip landSound;
    // 벽 슬라이드 사운드
    public AudioClip wallSlideSound;

    // ========================== 상태 변수 ==========================
    // 현재 점프 횟수 (이중 점프 제한용)
    private int jumpCount = 0;
    // 플레이어가 땅에 닿아 있는지 여부
    private bool isGrounded = false;
    // 플레이어가 오른쪽을 향하고 있는지 여부
    private bool facingRight = true;
    // 대시 중인지 여부
    private bool isDashing = false;
    // 대시 가능 여부
    private bool canDash = true;
    // 공중에서 대시를 사용했는지 여부
    private bool hasAirDashed = false;
    // 벽에 닿아 있는지 여부
    private bool isTouchingWall = false;
    // 벽 슬라이드 중인지 여부
    private bool isWallSliding = false;
    // 기본 이동 속도 저장용 (속도 감소 후 복구용)
    private float originalMoveSpeed;
    // 마지막 충돌면의 법선 벡터 (벽 감지 및 이동 제한용)
    private Vector2 lastContactNormal = Vector2.zero;

    // 무기 장착 상태
    [HideInInspector] public bool hasSword = false; // 검 장착 여부
    [HideInInspector] public bool canMove = true;   // 이동 가능 여부
    [HideInInspector] public bool isAttacking = false; // 공격 중인지 여부
    [HideInInspector] public bool hasLance = false; // 창 장착 여부
    [HideInInspector] public bool hasMace = false;  // 철퇴 장착 여부

    // ========================== 컴포넌트 참조 ==========================
    // 플레이어의 물리 컴포넌트
    private Rigidbody2D playerRigidbody;
    // 애니메이션 제어 컴포넌트
    private Animator animator;
    // 사운드 재생용 기본 오디오 소스
    private AudioSource playerAudio;
    // 걷기 및 벽 슬라이드 사운드용 별도 오디오 소스
    private AudioSource walkAudioSource;
    // 창 공격 스크립트
    private LanceAttack lanceAttack;

    // 현재 장착된 무기의 스탯
    public WeaponStats currentWeaponStats;
    // 플레이어의 체력 관리 스크립트
    public PlayerHealth playerHealth;

    // 컴포넌트 초기화 (Awake는 Start보다 먼저 호출됨)
    private void Awake()
    {
        // PlayerHealth 컴포넌트가 없으면 현재 게임오브젝트에서 가져옴
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();
        // LanceAttack 컴포넌트 가져오기
        lanceAttack = GetComponent<LanceAttack>();
    }

    // 초기 설정
    private void Start()
    {
        // Rigidbody2D 컴포넌트 가져오기
        playerRigidbody = GetComponent<Rigidbody2D>();
        // Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();
        // 기본 AudioSource 컴포넌트 가져오기
        playerAudio = GetComponent<AudioSource>();

        // 걷기 및 벽 슬라이드 사운드용 추가 AudioSource 생성
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.loop = true; // 루프 설정

        // 기본 이동 속도 저장
        originalMoveSpeed = moveSpeed;

        // 시작 시 UI 업데이트 (무기 없음 상태)
        if (statsUIManager != null)
        {
            statsUIManager.UpdateStatsUI(null);
        }
    }

    // 매 프레임 호출
    private void Update()
    {
        // 대시 중에는 다른 입력 처리 중지
        if (isDashing) return;

        // 벽 슬라이드 처리
        HandleWallSlide();
        // 이동 처리
        HandleMovement();
        // 점프 처리
        HandleJump();
        // 대시 처리
        HandleDash();

        // 애니메이터에 상태 업데이트
        animator.SetBool("Grounded", isGrounded); // 땅에 닿아 있는지
        animator.SetBool("HasSword", hasSword);   // 검 장착 여부
        animator.SetBool("HasLance", hasLance);   // 창 장착 여부
        animator.SetBool("HasMace", hasMace);     // 철퇴 장착 여부
    }

    // 플레이어 이동 처리
    private void HandleMovement()
    {
        // 이동 불가 상태에서는 이동 중지
        if (!canMove)
        {
            playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y); // x축 속도 0
            animator.SetFloat("Speed", 0); // 애니메이터 속도 파라미터 0
            return;
        }

        // 공중에 있고 벽에 붙어 있으면 이동 제한
        if (!isGrounded && Mathf.Abs(lastContactNormal.x) > 0.7f)
        {
            playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y); // x축 속도 0
            animator.SetFloat("Speed", 0); // 애니메이터 속도 파라미터 0
            return;
        }

        // 수평 입력 받기
        float move = Input.GetAxis("Horizontal");
        // 공격 중이면 공격용 속도, 아니면 기본 속도 사용
        float currentSpeed = isAttacking ? attackMoveSpeed : moveSpeed;
        // x축 속도 설정
        playerRigidbody.velocity = new Vector2(move * currentSpeed, playerRigidbody.velocity.y);

        // 방향 전환
        if (move > 0 && !facingRight) Flip();
        else if (move < 0 && facingRight) Flip();

        // 애니메이터에 이동 속도 전달
        animator.SetFloat("Speed", Mathf.Abs(move));
    }

    // 점프 처리
    private void HandleJump()
    {
        // 이동 불가 상태에서는 점프 불가
        if (!canMove) return;

        // K 키 입력 시 점프 가능 여부 확인
        if (Input.GetKeyDown(KeyCode.K) && jumpCount < 1)
        {
            jumpCount++; // 점프 횟수 증가
            playerRigidbody.velocity = Vector2.zero; // 기존 속도 초기화
            playerRigidbody.AddForce(new Vector2(0, jumpForce)); // 점프 힘 적용

            // 점프 사운드 재생
            if (jumpSound != null)
                playerAudio.PlayOneShot(jumpSound);

            // 점프 먼지 효과 생성
            if (jumpDustPrefab != null && dustSpawnPoint != null)
            {
                GameObject dust = Instantiate(jumpDustPrefab, dustSpawnPoint.position, Quaternion.identity);
                Destroy(dust, 0.4f); // 0.4초 후 제거
            }
        }
        // 점프 키 떼면 상승 속도 감소
        else if (Input.GetKeyUp(KeyCode.K) && playerRigidbody.velocity.y > 0)
        {
            playerRigidbody.velocity *= 0.5f; // 상승 속도 절반으로
        }
    }

    // 대시 처리
    private void HandleDash()
    {
        // 이동 불가 상태에서는 대시 불가
        if (!canMove) return;

        // L 키 입력 시 대시 가능 여부 확인
        if (Input.GetKeyDown(KeyCode.L) && canDash)
        {
            // 공중에서 이미 대시했으면 불가
            if (!isGrounded && hasAirDashed) return;

            // 대시 코루틴 실행
            StartCoroutine(Dash());
            // 공중 대시 플래그 설정
            if (!isGrounded) hasAirDashed = true;
        }
    }

    // 대시 동작 코루틴
    private IEnumerator Dash()
    {
        // 대시 상태 활성화
        isDashing = true;
        // 대시 불가 상태로 전환
        canDash = false;
        // 대시 애니메이션 활성화
        animator.SetBool("Dash", true);

        // 검 장착 시 대시 중 무적 상태 활성화 (깜빡임 비활성화)
        if (hasSword && playerHealth != null)
        {
            StartCoroutine(playerHealth.StartInvincibility(dashDuration, blink: false));
        }

        // 방향에 따라 대시 방향 설정
        Vector2 dashDirection = facingRight ? Vector2.right : Vector2.left;
        // 기존 속도 초기화
        playerRigidbody.velocity = Vector2.zero;
        // 대시 힘 적용
        playerRigidbody.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);

        // 무기에 따른 대시 사운드 선택
        AudioClip selectedDashSound = hasMace ? maceDashSound : (hasSword ? swordDashSound : (hasLance ? lanceDashSound : defaultDashSound));
        if (selectedDashSound != null)
            playerAudio.PlayOneShot(selectedDashSound); // 대시 사운드 재생

        // 대시 효과 생성
        if (dashEffectPrefab != null && dustSpawnPoint != null)
        {
            GameObject dashEffect = Instantiate(dashEffectPrefab, dustSpawnPoint.position, Quaternion.identity);
            Vector3 scale = dashEffect.transform.localScale;
            // 방향에 따라 효과 스케일 조정
            scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            dashEffect.transform.localScale = scale;
            Destroy(dashEffect, 0.5f); // 0.5초 후 제거
        }

        // 대시 지속 시간 대기
        yield return new WaitForSeconds(dashDuration);
        // 대시 상태 비활성화
        isDashing = false;
        // 대시 애니메이션 비활성화
        animator.SetBool("Dash", false);
        // 쿨다운 시간 대기
        yield return new WaitForSeconds(dashCooldown);
        // 대시 가능 상태로 복구
        canDash = true;
    }

    // 벽 슬라이드 처리
    private void HandleWallSlide()
    {
        // 벽 감지 (원형 오버랩 체크)
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);

        // 공중에 있고 벽에 닿아 있으며 하강 중이면 벽 슬라이드 활성화
        if (!isGrounded && isTouchingWall && playerRigidbody.velocity.y < 0)
        {
            isWallSliding = true;
            // 하강 속도를 벽 슬라이드 속도로 제한
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -wallSlideSpeed);
            // 벽 슬라이드 애니메이션 활성화
            animator.SetBool("Wall", true);
            // 점프 횟수 초기화
            jumpCount = 0;

            // 벽 슬라이드 사운드 재생
            if (wallSlideSound != null && !walkAudioSource.isPlaying)
            {
                walkAudioSource.clip = wallSlideSound;
                walkAudioSource.Play();
            }
        }
        else
        {
            // 벽 슬라이드 비활성화
            isWallSliding = false;
            // 벽 슬라이드 애니메이션 비활성화
            animator.SetBool("Wall", false);

            // 벽 슬라이드 사운드 중지
            if (walkAudioSource.clip == wallSlideSound)
                walkAudioSource.Stop();
        }

        // 벽 슬라이드 중 K 키로 벽 점프
        if (isWallSliding && Input.GetKeyDown(KeyCode.K))
        {
            // 벽 반대 방향으로 점프
            float wallJumpDir = facingRight ? -1 : 1;
            playerRigidbody.velocity = Vector2.zero; // 기존 속도 초기화
            playerRigidbody.AddForce(new Vector2(wallJumpDir * moveSpeed * 60f, wallJumpForce)); // 벽 점프 힘 적용
            Flip(); // 방향 전환
            isWallSliding = false; // 벽 슬라이드 비활성화
            animator.SetBool("Wall", false); // 벽 슬라이드 애니메이션 비활성화

            // 벽 슬라이드 사운드 중지
            if (walkAudioSource.clip == wallSlideSound)
                walkAudioSource.Stop();

            // 벽 점프 후 이동 제한 해제
            StartCoroutine(AllowHorizontalInputAfterWallJump());
        }
    }

    // 벽 점프 후 이동 제한 해제 코루틴
    private IEnumerator AllowHorizontalInputAfterWallJump()
    {
        // 0.1초 대기 후 충돌 법선 벡터 초기화
        yield return new WaitForSeconds(0.1f);
        lastContactNormal = Vector2.zero;
    }

    // 걷기 사운드 재생
    private void PlayWalkSound()
    {
        // 땅에 닿아 있을 때만 사운드 재생
        if (walkSound != null && isGrounded)
            playerAudio.PlayOneShot(walkSound);
    }

    // 트리거 충돌 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 적과 충돌 시 속도 감소 처리
        if (other.CompareTag("Enemy"))
            StartCoroutine(HandleEnemyCollision());
    }

    // 충돌 시작 처리
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 모든 접촉점 확인
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // 접촉면이 바닥(위쪽 법선)일 경우
            if (contact.normal.y > 0.7f)
            {
                isGrounded = true; // 땅에 닿음
                jumpCount = 0; // 점프 횟수 초기화
                hasAirDashed = false; // 공중 대시 초기화
                // 착지 사운드 재생
                if (landSound != null)
                    playerAudio.PlayOneShot(landSound);
                break;
            }
        }
    }

    // 충돌 유지 중 처리
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 첫 번째 접촉점의 법선 벡터 저장
        foreach (ContactPoint2D contact in collision.contacts)
        {
            lastContactNormal = contact.normal;
            break;
        }
    }

    // 충돌 종료 처리
    private void OnCollisionExit2D(Collision2D collision)
    {
        // 여전히 바닥에 닿아 있는지 확인
        bool stillGrounded = false;
        ContactPoint2D[] contacts = new ContactPoint2D[10];
        int contactCount = playerRigidbody.GetContacts(contacts);

        for (int i = 0; i < contactCount; i++)
        {
            if (contacts[i].normal.y > 0.7f)
            {
                stillGrounded = true;
                break;
            }
        }

        isGrounded = stillGrounded; // 바닥 상태 업데이트
    }

    // 적 충돌 시 속도 감소 코루틴
    private IEnumerator HandleEnemyCollision()
    {
        // 이동 속도 감소
        moveSpeed *= speedReductionFactor;
        // 지속 시간 대기
        yield return new WaitForSeconds(speedReductionDuration);
        // 원래 속도로 복구
        moveSpeed = originalMoveSpeed;
    }

    // 플레이어 방향 전환
    private void Flip()
    {
        // 방향 반전
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        // x축 스케일 반전
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    // 검 장착
    public void EquipSword(WeaponStats newStats)
    {
        hasSword = true; // 검 장착 상태 활성화
        currentWeaponStats = newStats; // 무기 스탯 업데이트

        // 무기 스탯 적용
        if (currentWeaponStats != null)
        {
            moveSpeed = currentWeaponStats.moveSpeed; // 이동 속도
            originalMoveSpeed = moveSpeed; // 기본 속도 저장
            dashForce = currentWeaponStats.dashForce; // 대시 힘
            dashCooldown = currentWeaponStats.dashCooldown; // 대시 쿨다운
            dashDuration = currentWeaponStats.dashDuration; // 대시 지속 시간
            if (playerHealth != null)
                playerHealth.ApplyBonusHealth(currentWeaponStats.bonusHealth); // 보너스 체력 적용
        }

        // 검 장착 사운드 재생
        if (swordEquipSound != null)
            playerAudio.PlayOneShot(swordEquipSound);

        // UI 업데이트
        if (statsUIManager != null)
            statsUIManager.UpdateStatsUI(currentWeaponStats);
    }

    // 창 장착
    public void EquipLance(WeaponStats newStats)
    {
        hasLance = true; // 창 장착 상태 활성화
        currentWeaponStats = newStats; // 무기 스탯 업데이트

        // 무기 스탯 적용
        if (currentWeaponStats != null)
        {
            moveSpeed = currentWeaponStats.moveSpeed;
            originalMoveSpeed = moveSpeed;
            dashForce = currentWeaponStats.dashForce;
            dashCooldown = currentWeaponStats.dashCooldown;
            dashDuration = currentWeaponStats.dashDuration;
            if (playerHealth != null)
                playerHealth.ApplyBonusHealth(currentWeaponStats.bonusHealth);
        }

        // 창 장착 사운드 재생
        if (lanceEquipSound != null)
            playerAudio.PlayOneShot(lanceEquipSound);

        // UI 업데이트
        if (statsUIManager != null)
            statsUIManager.UpdateStatsUI(currentWeaponStats);
    }

    // 철퇴 장착
    public void EquipMace(WeaponStats newStats)
    {
        hasMace = true; // 철퇴 장착 상태 활성화
        currentWeaponStats = newStats; // 무기 스탯 업데이트

        // 무기 스탯 적용
        if (currentWeaponStats != null)
        {
            moveSpeed = currentWeaponStats.moveSpeed;
            originalMoveSpeed = moveSpeed;
            dashForce = currentWeaponStats.dashForce;
            dashCooldown = currentWeaponStats.dashCooldown;
            dashDuration = currentWeaponStats.dashDuration;
            if (playerHealth != null)
                playerHealth.ApplyBonusHealth(currentWeaponStats.bonusHealth);
        }

        // 철퇴 장착 사운드 재생
        if (maceEquipSound != null)
            playerAudio.PlayOneShot(maceEquipSound);

        // UI 업데이트
        if (statsUIManager != null)
            statsUIManager.UpdateStatsUI(currentWeaponStats);
    }

    // 대시 상태 반환
    public bool IsDashing()
    {
        return isDashing;
    }
}