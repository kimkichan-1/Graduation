// PlayerMovement.cs (2D 버전)

using UnityEngine;

// Rigidbody2D 컴포넌트가 반드시 필요하다고 명시
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 상하좌우 입력을 받음
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // 물리 업데이트는 FixedUpdate에서 처리하는 것이 안정적
        rb.linearVelocity = moveInput * speed;
    }
}