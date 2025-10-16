using UnityEngine;

/// <summary>
/// 월드에 배치되는 스탯 부스트 아이템 픽업
/// ItemPickup과 유사하지만 즉시 사용되는 방식
/// </summary>
public class StatBoostItemPickup : MonoBehaviour
{
    [Header("아이템 설정")]
    public StatBoostItemData statBoostItem; // 스탯 부스트 아이템 데이터
    public bool addToInventory = false; // true: 인벤토리에 추가, false: 즉시 사용

    [Header("비주얼 설정")]
    public SpriteRenderer spriteRenderer;
    public GameObject pickupEffectPrefab; // 획득 시 이펙트
    public AudioClip pickupSound;

    [Header("물리 설정")]
    [SerializeField] private LayerMask groundLayer; // 바닥 레이어
    [SerializeField] private float groundCheckDistance = 0.5f; // 바닥 체크 거리
    [SerializeField] private float gravityForce = 9.8f; // 중력 힘
    [SerializeField] private float drag = 2f; // 공기 저항

    private Rigidbody2D rb;
    private Vector2 velocity;
    private bool isGrounded = false;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // AudioSource가 없으면 추가
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 아이템 데이터가 있으면 스프라이트 설정
        if (statBoostItem != null && spriteRenderer != null && statBoostItem.icon != null)
        {
            spriteRenderer.sprite = statBoostItem.icon;
        }

        // Rigidbody2D가 있으면 초기 속도 설정
        if (rb != null)
        {
            velocity = rb.linearVelocity;
            // Continuous Collision Detection 설정으로 빠른 통과 방지
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    void FixedUpdate()
    {
        if (rb == null || isGrounded) return;

        // 레이캐스트로 바닥 체크
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        if (hit.collider != null)
        {
            // 바닥에 닿았을 때
            float distanceToGround = hit.distance;

            if (distanceToGround < 0.1f && Mathf.Abs(velocity.y) < 0.5f)
            {
                // 완전히 멈춤
                isGrounded = true;
                velocity = Vector2.zero;
                rb.linearVelocity = Vector2.zero;

                // Kinematic으로 변경하여 더 이상 물리 영향을 받지 않도록
                rb.bodyType = RigidbodyType2D.Kinematic;

                // 바닥 위치로 정렬
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }
            else if (velocity.y < 0)
            {
                // 바운스 효과
                velocity.y = Mathf.Abs(velocity.y) * 0.3f; // 30% 반발
            }
        }
        else
        {
            // 공중에 있을 때 중력 적용
            velocity.y -= gravityForce * Time.fixedDeltaTime;
        }

        // 수평 속도 감소 (공기 저항)
        velocity.x = Mathf.Lerp(velocity.x, 0, drag * Time.fixedDeltaTime);

        // Rigidbody에 속도 적용
        rb.linearVelocity = velocity;
        velocity = rb.linearVelocity;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 닿았을 때
        if (other.CompareTag("Player"))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        if (statBoostItem == null)
        {
            Debug.LogWarning("StatBoostItem 데이터가 설정되지 않았습니다!");
            return;
        }

        Debug.Log($"{statBoostItem.itemName} 획득!");

        // 인벤토리에 추가할지, 즉시 사용할지 결정
        if (addToInventory)
        {
            // 인벤토리에 추가
            bool wasPickedUp = Inventory.instance.Add(statBoostItem);
            if (!wasPickedUp)
            {
                Debug.Log("인벤토리가 가득 찼습니다!");
                return;
            }
        }
        else
        {
            // 즉시 사용 (영구 스탯 증가)
            statBoostItem.Use();
        }

        // 획득 사운드 재생
        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        // 획득 이펙트 생성
        if (pickupEffectPrefab != null)
        {
            GameObject effect = Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // 오브젝트 파괴
        Destroy(gameObject);
    }

    // 디버그용 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
