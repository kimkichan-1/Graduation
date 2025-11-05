using UnityEngine;

public class BackGround : MonoBehaviour
{
    [SerializeField]
    private Transform target; // 캐릭터 또는 추적 대상
    [SerializeField]
    private float scrollAmount; // 배경이 이동할 거리 (재배치 기준)
    [SerializeField]
    private float moveSpeed; // 배경 이동 속도
    [SerializeField]
    private Vector3 moveDirection = Vector3.left; // 이동 방향 (기본값: 왼쪽)

    private void Update()
    {
        // 배경을 지정된 방향으로 이동
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 배경의 x 위치가 scrollAmount보다 작아지면 재배치
        if (transform.position.x <= scrollAmount)
        {
            // target의 위치를 기준으로 배경을 재배치
            transform.position = target.position - moveDirection * scrollAmount;
        }
    }
}