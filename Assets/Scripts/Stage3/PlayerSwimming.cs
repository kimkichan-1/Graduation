using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSwimming : MonoBehaviour
{
    [Header("헤엄 설정")]
    public float swimSpeed = 4f;
    public float ascendForce = 5.5f;
    public float waterDrag = 1.5f;
    public float gravityScaleInWater = 1f;

    [Header("벽 감지 설정")]
    public Transform wallCheck;
    public float wallCheckRadius = 0.1f;
    public LayerMask wallLayer;

    // --- ▼▼▼▼▼ 수정된 부분 (점프 카운트 관련) ▼▼▼▼▼ ---
    [Header("상승 제한")]
    [SerializeField] private int maxAscendCount = 5; // 물 속에서 연속 상승 가능한 횟수 (5회로 변경)
    private int currentAscendCount = 0;
    // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---

    // --- ▼▼▼▼▼ 추가된 부분 (땅 감지 관련) ▼▼▼▼▼ ---
    [Header("땅 감지 설정")]
    [SerializeField] private Transform groundCheck; // 플레이어 발밑 위치 (빈 오브젝트 연결)
    [SerializeField] private float groundCheckRadius = 0.2f; // 땅 감지 범위
    [SerializeField] private LayerMask groundLayer; // 땅으로 인식할 레이어
    private bool isGrounded = false; // 현재 땅에 닿아있는지 여부
    // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---

    private Rigidbody2D rb;
    private float originalGravityScale;
    private float moveInput;

    private PhysicsMaterial2D noFrictionMaterial;
    private PhysicsMaterial2D originalMaterial;
    private PlayerController playerController;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();

        if (wallCheck == null) wallCheck = transform.Find("WallCheck");
        // --- ▼▼▼▼▼ 추가된 부분 (GroundCheck 자동 찾기) ▼▼▼▼▼ ---
        if (groundCheck == null) groundCheck = transform.Find("GroundCheck");
        // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---

        noFrictionMaterial = new PhysicsMaterial2D("NoFriction_Swim");
        noFrictionMaterial.friction = 0f;
        noFrictionMaterial.bounciness = 0f;
    }

    void OnEnable()
    {
        originalGravityScale = rb.gravityScale;
        rb.gravityScale = gravityScaleInWater;
        rb.linearDamping = waterDrag;
        originalMaterial = rb.sharedMaterial;
        rb.sharedMaterial = noFrictionMaterial;
        currentAscendCount = 0; // 활성화 시 카운트 초기화
    }

    void OnDisable()
    {
        rb.gravityScale = originalGravityScale;
        rb.linearDamping = 0f;
        rb.sharedMaterial = originalMaterial;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // 'K'키를 누르고, 아직 최대 상승 횟수에 도달하지 않았을 때
        if (Input.GetKeyDown(KeyCode.K) && currentAscendCount < maxAscendCount)
        {
            currentAscendCount++;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.8f);
            rb.AddForce(Vector2.up * ascendForce, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (playerController != null && playerController.IsDashing())
        {
            return;
        }

        // --- ▼▼▼▼▼ 추가된 부분 (땅 감지 로직) ▼▼▼▼▼ ---
        // 매 프레임 땅에 닿아있는지 확인
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---

        bool isTouchingWall = false;
        if (wallCheck != null)
        {
            isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
        }

        float currentMoveSpeed = swimSpeed;

        if (isTouchingWall && ((moveInput > 0 && transform.localScale.x > 0) || (moveInput < 0 && transform.localScale.x < 0)))
        {
            currentMoveSpeed = 0;
        }

        rb.velocity = new Vector2(moveInput * currentMoveSpeed, rb.velocity.y);

        // --- ▼▼▼▼▼ 수정된 부분 (상승 횟수 초기화 조건 변경) ▼▼▼▼▼ ---
        // 플레이어가 땅(Ground)에 착지하면 상승 횟수를 초기화합니다.
        if (isGrounded)
        {
            currentAscendCount = 0;
        }
        // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---
    }

    // --- ▼▼▼▼▼ 추가된 부분 (땅 감지 범위 시각화) ▼▼▼▼▼ ---
    // Scene 뷰에서 땅 감지 범위를 원으로 표시 (디버깅용)
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---
}