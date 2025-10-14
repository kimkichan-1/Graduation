using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSwimming : MonoBehaviour
{
    [Header("헤엄 설정")]
    public float swimSpeed = 4f;     // 좌우 이동 속도
    public float ascendForce = 5.5f;  // K키를 눌렀을 때 솟구치는 힘
    public float waterDrag = 1.5f;     // 물의 저항
    public float gravityScaleInWater = 1f; // 물 속에서의 중력

    [Header("벽 감지 설정")]
    public Transform wallCheck;
    public float wallCheckRadius = 0.1f;
    public LayerMask wallLayer;

    private Rigidbody2D rb;
    private float originalGravityScale;
    private float moveInput; // 수평 입력 값 저장

    private PhysicsMaterial2D noFrictionMaterial;
    private PhysicsMaterial2D originalMaterial;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 이 스크립트가 시작될 때, 플레이어의 자식 중 "WallCheck"를 찾아 자동으로 연결합니다.
        if (wallCheck == null)
        {
            wallCheck = transform.Find("WallCheck");
        }

        // 마찰력 없는 물리 재질 생성
        noFrictionMaterial = new PhysicsMaterial2D("NoFriction_Swim");
        noFrictionMaterial.friction = 0f;
        noFrictionMaterial.bounciness = 0f;
    }

    // 이 스크립트가 활성화될 때 (Stage3Manager가 켜주는 시점)
    void OnEnable()
    {
        // 원래 물리 값을 저장하고, 물 속 값으로 변경
        originalGravityScale = rb.gravityScale;
        rb.gravityScale = gravityScaleInWater;
        rb.linearDamping = waterDrag;

        originalMaterial = rb.sharedMaterial;
        rb.sharedMaterial = noFrictionMaterial;
    }

    // 이 스크립트가 비활성화될 때 (다른 스테이지로 갈 때)
    void OnDisable()
    {
        // 원래 물리 값으로 복구
        rb.gravityScale = originalGravityScale;
        rb.linearDamping = 0f;
        rb.sharedMaterial = originalMaterial;
    }

    void Update()
    {
        // 좌우 이동 입력을 받습니다.
        moveInput = Input.GetAxisRaw("Horizontal");

        // ★★★ 기존 로직 유지: 'K'키로 상승 ★★★
        // GetKeyDown은 한 번 눌렀을 때를 감지합니다.
        if (Input.GetKeyDown(KeyCode.K))
        {
            // 위쪽으로 순간적인 힘(Impulse)을 가해 솟구치게 합니다.
            rb.AddForce(Vector2.up * ascendForce, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        // 벽 감지
        bool isTouchingWall = false;
        if (wallCheck != null)
        {
            isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
        }

        float currentMoveSpeed = swimSpeed;

        // 벽에 닿았고, 그 방향으로 계속 이동하려고 하면 수평 이동 속도를 0으로 만듦
        if (isTouchingWall && ((moveInput > 0 && transform.localScale.x > 0) || (moveInput < 0 && transform.localScale.x < 0)))
        {
            currentMoveSpeed = 0;
        }

        // ★★★ 기존 로직 유지: 반응성 좋은 좌우 이동 ★★★
        // AddForce 대신, velocity를 직접 제어하여 즉각적인 움직임을 만듭니다.
        rb.linearVelocity = new Vector2(moveInput * currentMoveSpeed, rb.linearVelocity.y);
    }
}