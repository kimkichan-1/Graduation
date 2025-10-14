using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData item; // The specific item data for this pickup

    [Header("물리 설정")]
    [SerializeField] private LayerMask groundLayer; // 바닥 레이어
    [SerializeField] private float groundCheckDistance = 0.5f; // 바닥 체크 거리
    [SerializeField] private float gravityForce = 9.8f; // 중력 힘
    [SerializeField] private float bounceForce = 3f; // 바운스 힘
    [SerializeField] private float drag = 2f; // 공기 저항

    private Rigidbody2D rb;
    private Vector2 velocity;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Rigidbody2D가 있으면 초기 속도 설정
        if (rb != null)
        {
            velocity = rb.linearVelocity;
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
        // Check if the object that entered the trigger is the player
        if (other.CompareTag("Player"))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        Debug.Log("Picking up " + item.itemName);
        // Add to inventory
        bool wasPickedUp = Inventory.instance.Add(item);

        // If successfully picked up, destroy the item from the scene
        if (wasPickedUp)
        {
            Destroy(gameObject);
        }
    }

    // 디버그용 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
