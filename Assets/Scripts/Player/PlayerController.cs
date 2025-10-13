using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(CharacterStats))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("플레이어 기본 스탯 (원본 데이터)")]
    public int attackPower = 10;
    public int defensePower = 5;
    public float baseMoveSpeed = 5f;

    [Header("UI 참조")]
    public StatsUIManager statsUIManager;
    private PlayerStats playerStats;
    private PlayerHealth playerHealth;
    private CharacterStats characterStats;

    [Header("운동 설정")]
    public float moveSpeed;
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

    private int jumpCount = 0;
    private bool isGrounded = false;
    private bool facingRight = true;
    private bool isDashing = false;
    private bool canDash = true;
    private bool hasAirDashed = false;
    private bool isTouchingWall = false;
    private bool isWallSliding = false;
    private float originalMoveSpeed;
    private Vector2 lastContactNormal = Vector2.zero;

    [HideInInspector] public bool hasSword, canMove = true, isAttacking, hasLance, hasMace;

    private Rigidbody2D playerRigidbody;
    private Animator animator;
    private AudioSource playerAudio;
    private AudioSource walkAudioSource;
    private LanceAttack lanceAttack;

    public WeaponStats currentWeaponStats;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        playerHealth = GetComponent<PlayerHealth>();
        playerStats = GetComponent<PlayerStats>();
        characterStats = GetComponent<CharacterStats>();
        lanceAttack = GetComponent<LanceAttack>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
    }

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
        PlayerSpawnPoint spawnPoint = FindFirstObjectByType<PlayerSpawnPoint>();
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
        }
    }

    private void Start()
    {
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.loop = true;
        
        RecalculateStats();
        UpdateAllStatsUI();
    }

    private void Update()
    {
        if (BossGameManager.Instance != null && BossGameManager.Instance.currentState == GameState.Battle)
        {
            if (playerRigidbody.linearVelocity.sqrMagnitude > 0)
            {
                playerRigidbody.linearVelocity = Vector2.zero;
                if(animator != null) animator.SetFloat("Speed", 0f);
            }
            return;
        }
        
        if (isDashing) return;
        
        if (turnManager != null)
        {
            if (isPlanning)
            {
                HandleAttackInput();
            }

            if (canExecuteTurn && Input.GetKeyDown(KeyCode.Space))
            {
                turnManager.StartTurnExecution();
            }

            if (!isPlanning)
            {
                playerRigidbody.linearVelocity = Vector2.zero;
                if(animator != null) animator.SetFloat("Speed", 0f);
                return;
            }
        }
        
        if (isDashing) return;
        HandleWallSlide();
        HandleMovement();
        HandleJump();
        HandleDash();
        if(animator != null) 
        {
            animator.SetBool("Grounded", isGrounded);
            animator.SetBool("HasSword", hasSword);
            animator.SetBool("HasLance", hasLance);
            animator.SetBool("HasMace", hasMace);
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (isPlanning)
            {
                actionBuffer.Add(attackAction);
                Debug.Log("공격 행동을 버퍼에 추가했습니다.");
            }
        }
    }

    private void HandleMovement()
    {
        
        if (!canMove)
        {
            playerRigidbody.linearVelocity = new Vector2(0, playerRigidbody.linearVelocity.y);
            if(animator != null) animator.SetFloat("Speed", 0);
            return;
        }
        if (!isGrounded && Mathf.Abs(lastContactNormal.x) > 0.7f)
        {
            playerRigidbody.linearVelocity = new Vector2(0, playerRigidbody.linearVelocity.y);
            if(animator != null) animator.SetFloat("Speed", 0);
            return;
        }
        float move = Input.GetAxis("Horizontal");
        float currentSpeed = isAttacking ? attackMoveSpeed : moveSpeed;
        playerRigidbody.linearVelocity = new Vector2(move * currentSpeed, playerRigidbody.linearVelocity.y);
        if (move > 0 && !facingRight) Flip();
        else if (move < 0 && facingRight) Flip();
        if(animator != null) animator.SetFloat("Speed", Mathf.Abs(move));
    }

    private void HandleJump()
    {
        
        if (!canMove) return;
        if (Input.GetKeyDown(KeyCode.K) && jumpCount < 1)
        {
            jumpCount++;
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
            if (jumpSound != null) playerAudio.PlayOneShot(jumpSound);
            if (jumpDustPrefab != null && dustSpawnPoint != null) Destroy(Instantiate(jumpDustPrefab, dustSpawnPoint.position, Quaternion.identity), 0.4f);
        }
        else if (Input.GetKeyUp(KeyCode.K) && playerRigidbody.linearVelocity.y > 0)
        {
            playerRigidbody.linearVelocity *= 0.5f;
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
        if(animator != null) animator.SetBool("Dash", true);
        if (hasSword && playerHealth != null) StartCoroutine(playerHealth.StartInvincibility(dashDuration, blink: false));
        Vector2 dashDirection = facingRight ? Vector2.right : Vector2.left;
        playerRigidbody.linearVelocity = Vector2.zero;
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
        if(animator != null) animator.SetBool("Dash", false);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void HandleWallSlide()
    {
        if (wallCheck != null)
        {
            isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
        }
        
        if (!isGrounded && isTouchingWall && playerRigidbody.linearVelocity.y < 0)
        {
            isWallSliding = true;
            playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, -wallSlideSpeed);
            if(animator != null) animator.SetBool("Wall", true);
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
            if(animator != null) animator.SetBool("Wall", false);
            if (walkAudioSource.clip == wallSlideSound) walkAudioSource.Stop();
        }

        if (isWallSliding && Input.GetKeyDown(KeyCode.K))
        {
            float wallJumpDir = facingRight ? -1 : 1;
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.AddForce(new Vector2(wallJumpDir * moveSpeed * 60f, wallJumpForce));
            Flip();
            isWallSliding = false;
            if(animator != null) animator.SetBool("Wall", false);
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
        moveSpeed = originalMoveSpeed;
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
        
        RecalculateStats();
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
        if (playerStats != null)
        {
            moveSpeed = finalMoveSpeed + playerStats.bonusMoveSpeed;
        }
        else
        {
            moveSpeed = finalMoveSpeed;
        }
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

    private void PerformMaceDashKnockback()
    {
        LayerMask monsterLayer = LayerMask.GetMask("Enemy");
        float staggerDuration = 0.5f;

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

    // ========================== 턴 기반 시스템 변수 ==========================
    [Header("Turn-Based System")]
    public ActionData attackAction;
    private List<ActionData> actionBuffer = new List<ActionData>();
    private bool isPlanning = false;
    private bool canExecuteTurn = false;
    private TurnManager turnManager;

    // ========================== 턴 관련 메서드 ==========================
    public void StartPlanningPhase()
    {
        isPlanning = true;
        canExecuteTurn = true;
        actionBuffer.Clear();
        Debug.Log("플레이어 턴 계획 시작.");
    }
    
    public void ExecuteTurn()
    {
        isPlanning = false;
        canExecuteTurn = false;
        StartCoroutine(ExecuteActions());
    }
    
    private IEnumerator ExecuteActions()
    {
        foreach (var action in actionBuffer)
        {
            if (action.type == ActionData.ActionType.Attack)
            {
                if (hasSword)
                    StartCoroutine(GetComponent<Sword>()?.Attack1());
                else if (hasMace)
                    StartCoroutine(GetComponent<Mace>()?.Attack1());
                else if (hasLance)
                    StartCoroutine(GetComponent<Lance>()?.Attack1());
                
                yield return new WaitForSeconds(action.duration);
            }
        }
        
        if (TurnManager.instance != null)
        {
            TurnManager.instance.OnPlayerActionsComplete();
        }
    }
}