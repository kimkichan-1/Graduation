using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // ========================== UI 참조 ==========================
    public StatsUIManager statsUIManager;
    private PlayerStats playerStats;
    private PlayerHealth playerHealth;

    // ========================== 설정 값 ==========================
    [Header("운동 설정")]
    public float baseMoveSpeed = 5f; // 플레이어의 기본 이동 속도
    public float moveSpeed; // 실제 이동에 사용될 최종 이동 속도
    public float jumpForce = 300f;
    public float wallJumpForce = 300f;
    public float attackMoveSpeed = 1f;
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("적 트리거 설정")]
    public float speedReductionFactor = 0.5f;
    public float speedReductionDuration = 0.5f;

    [Header("이펙트 설정")]
    public GameObject jumpDustPrefab;
    public Transform dustSpawnPoint;
    public GameObject dashEffectPrefab;

    [Header("벽 슬라이드 설정")]
    public float wallSlideSpeed = 1f;
    public Transform wallCheck;
    public float wallCheckRadius = 0.1f;
    public LayerMask wallLayer;

    [Header("사운드 설정")]
    public AudioClip jumpSound, walkSound, defaultDashSound, swordDashSound, maceDashSound, lanceDashSound, swordEquipSound, lanceEquipSound, maceEquipSound, landSound, wallSlideSound;

    [Header("메이스 대쉬 설정")]
    public float maceKnockbackRadius = 2.0f;
    public float maceKnockbackForce = 15f;

    // ========================== 상태 변수 ==========================
    private int jumpCount = 0;
    private bool isGrounded = false;
    private bool facingRight = true;
    private bool isDashing = false;
    private bool canDash = true;
    private bool hasAirDashed = false;
    private bool isTouchingWall = false;
    private bool isWallSliding = false;
    private float originalMoveSpeed; // 속도 감소 효과에 사용될 임시 저장 변수
    private Vector2 lastContactNormal = Vector2.zero;

    [HideInInspector] public bool hasSword, canMove = true, isAttacking, hasLance, hasMace;

    // ========================== 컴포넌트 참조 ==========================
    private Rigidbody2D playerRigidbody;
    private Animator animator;
    private AudioSource playerAudio;
    private AudioSource walkAudioSource;
    private LanceAttack lanceAttack;

    public WeaponStats currentWeaponStats;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬에 PlayerSpawnPoint가 있는지 찾습니다.
        PlayerSpawnPoint spawnPoint = FindObjectOfType<PlayerSpawnPoint>();
        if (spawnPoint != null)
        {
            // 스폰 포인트가 있으면 플레이어를 그 위치로 이동시킵니다.
            transform.position = spawnPoint.transform.position;
        }
    }

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerStats = GetComponent<PlayerStats>();
        lanceAttack = GetComponent<LanceAttack>();
    }

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.loop = true;
        
        RecalculateStats(); // 초기 스탯 계산
        UpdateAllStatsUI();
    }

    private void Update()
    {
        if (isDashing) return;
        HandleWallSlide();
        HandleMovement();
        HandleJump();
        HandleDash();
        animator.SetBool("Grounded", isGrounded);
        animator.SetBool("HasSword", hasSword);
        animator.SetBool("HasLance", hasLance);
        animator.SetBool("HasMace", hasMace);
    }

    private void HandleMovement()
    {
        if (!canMove)
        {
            playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
            animator.SetFloat("Speed", 0);
            return;
        }
        if (!isGrounded && Mathf.Abs(lastContactNormal.x) > 0.7f)
        {
            playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
            animator.SetFloat("Speed", 0);
            return;
        }
        float move = Input.GetAxis("Horizontal");
        float currentSpeed = isAttacking ? attackMoveSpeed : moveSpeed;
        playerRigidbody.velocity = new Vector2(move * currentSpeed, playerRigidbody.velocity.y);
        if (move > 0 && !facingRight) Flip();
        else if (move < 0 && facingRight) Flip();
        animator.SetFloat("Speed", Mathf.Abs(move));
    }

    private void HandleJump()
    {
        if (!canMove) return;
        if (Input.GetKeyDown(KeyCode.K) && jumpCount < 1)
        {
            jumpCount++;
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
            if (jumpSound != null) playerAudio.PlayOneShot(jumpSound);
            if (jumpDustPrefab != null && dustSpawnPoint != null) Destroy(Instantiate(jumpDustPrefab, dustSpawnPoint.position, Quaternion.identity), 0.4f);
        }
        else if (Input.GetKeyUp(KeyCode.K) && playerRigidbody.velocity.y > 0)
        {
            playerRigidbody.velocity *= 0.5f;
        }
    }

    private void HandleDash()
    {
        if (!canMove) return;
        if (Input.GetKeyDown(KeyCode.L) && canDash)
        {
            if (!isGrounded && hasAirDashed) return;
            StartCoroutine(Dash());
            if (!isGrounded) hasAirDashed = true;
        }
    }

    private IEnumerator Dash()
    {
        if (hasMace)
        {
            PerformMaceDashKnockback();
        }

        isDashing = true;
        canDash = false;
        animator.SetBool("Dash", true);
        if (hasSword && playerHealth != null) StartCoroutine(playerHealth.StartInvincibility(dashDuration, blink: false));
        Vector2 dashDirection = facingRight ? Vector2.right : Vector2.left;
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);
        AudioClip selectedDashSound = hasMace ? maceDashSound : (hasSword ? swordDashSound : (hasLance ? lanceDashSound : defaultDashSound));
        if (selectedDashSound != null) playerAudio.PlayOneShot(selectedDashSound);
        if (dashEffectPrefab != null && dustSpawnPoint != null)
        {
            GameObject dashEffect = Instantiate(dashEffectPrefab, dustSpawnPoint.position, Quaternion.identity);
            Vector3 scale = dashEffect.transform.localScale;
            scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            dashEffect.transform.localScale = scale;
            Destroy(dashEffect, 0.5f);
        }
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        animator.SetBool("Dash", false);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void HandleWallSlide()
    {
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
        if (!isGrounded && isTouchingWall && playerRigidbody.velocity.y < 0)
        {
            isWallSliding = true;
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -wallSlideSpeed);
            animator.SetBool("Wall", true);
            jumpCount = 0;
            if (wallSlideSound != null && !walkAudioSource.isPlaying)
            {
                walkAudioSource.clip = wallSlideSound;
                walkAudioSource.Play();
            }
        }
        else
        {
            isWallSliding = false;
            animator.SetBool("Wall", false);
            if (walkAudioSource.clip == wallSlideSound) walkAudioSource.Stop();
        }
        if (isWallSliding && Input.GetKeyDown(KeyCode.K))
        {
            float wallJumpDir = facingRight ? -1 : 1;
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.AddForce(new Vector2(wallJumpDir * moveSpeed * 60f, wallJumpForce));
            Flip();
            isWallSliding = false;
            animator.SetBool("Wall", false);
            if (walkAudioSource.clip == wallSlideSound) walkAudioSource.Stop();
            StartCoroutine(AllowHorizontalInputAfterWallJump());
        }
    }

    private IEnumerator AllowHorizontalInputAfterWallJump()
    {
        yield return new WaitForSeconds(0.1f);
        lastContactNormal = Vector2.zero;
    }

    private void PlayWalkSound()
    {
        if (walkSound != null && isGrounded) playerAudio.PlayOneShot(walkSound);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) StartCoroutine(HandleEnemyCollision());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.7f)
            {
                isGrounded = true;
                jumpCount = 0;
                hasAirDashed = false;
                if (landSound != null) playerAudio.PlayOneShot(landSound);
                break;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            lastContactNormal = contact.normal;
            break;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
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
        isGrounded = stillGrounded;
    }

    private IEnumerator HandleEnemyCollision()
    {
        moveSpeed *= speedReductionFactor;
        yield return new WaitForSeconds(speedReductionDuration);
        moveSpeed = originalMoveSpeed; // 원래 속도로 복구
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void EquipSword(WeaponStats newStats) { hasSword = true; EquipWeapon(newStats, swordEquipSound); }
    public void EquipLance(WeaponStats newStats) { hasLance = true; EquipWeapon(newStats, lanceEquipSound); }
    public void EquipMace(WeaponStats newStats) { hasMace = true; EquipWeapon(newStats, maceEquipSound); }

    private void EquipWeapon(WeaponStats newStats, AudioClip equipSound)
    {
        currentWeaponStats = newStats;
        if (playerHealth != null && currentWeaponStats != null) 
            playerHealth.ApplyBonusHealth(currentWeaponStats.bonusHealth);
        
        if (equipSound != null) playerAudio.PlayOneShot(equipSound);
        
        RecalculateStats(); // 무기 장착 후 스탯 재계산
        UpdateAllStatsUI();
    }

    public void RecalculateStats()
    {
        float weaponMoveSpeed = 0f;
        if (currentWeaponStats != null)
        {
            weaponMoveSpeed = currentWeaponStats.moveSpeed;
            dashForce = currentWeaponStats.dashForce;
            dashCooldown = currentWeaponStats.dashCooldown;
            dashDuration = currentWeaponStats.dashDuration;
        }

        float finalMoveSpeed = (weaponMoveSpeed > 0) ? weaponMoveSpeed : baseMoveSpeed;
        moveSpeed = finalMoveSpeed + playerStats.bonusMoveSpeed;
        originalMoveSpeed = moveSpeed;
    }

    public void UpdateAllStatsUI()
    {
        if (statsUIManager != null && playerStats != null && playerHealth != null)
        {
            statsUIManager.UpdateStatsUI(currentWeaponStats, playerStats, this, playerHealth);
        }
    }

    public bool IsDashing() { return isDashing; }

    // 메이스 대쉬 넉백 기능
    private void PerformMaceDashKnockback()
    {
        LayerMask monsterLayer = LayerMask.GetMask("Enemy");
        float staggerDuration = 0.5f; // 넉백 시 몬스터가 경직될 시간

        Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(transform.position, maceKnockbackRadius, monsterLayer);

        foreach (Collider2D monsterCollider in hitMonsters)
        {
            MonsterController monster = monsterCollider.GetComponent<MonsterController>();
            if (monster != null)
            {
                Vector2 knockbackDirection = (monster.transform.position - transform.position).normalized;
                monster.ApplyKnockback(knockbackDirection, maceKnockbackForce, staggerDuration);
            }
        }
    }
}