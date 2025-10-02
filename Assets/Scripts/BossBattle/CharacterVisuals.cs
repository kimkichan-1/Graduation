// CharacterVisuals.cs (2D 버전)

using System.Collections;
using UnityEngine;

public class CharacterVisuals : MonoBehaviour
{
    private Vector3 homePosition;
    private SpriteRenderer spriteRenderer; // 3D 모델 대신 스프라이트 렌더러

    void Start()
    {
        homePosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // MoveToPosition과 ReturnToHomePosition은 Vector3를 그대로 사용해도
    // 2D 환경에서 문제 없이 작동하므로 수정할 필요가 없습니다.
    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        // 이동 시 타겟을 바라보도록 방향 전환
        FaceTarget(targetPosition);

        Vector3 startPosition = transform.position;
        float time = 0;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    public IEnumerator ReturnToHomePosition(float duration)
    {
        // 복귀 시 원래 위치를 바라보도록 방향 전환
        FaceTarget(homePosition);
        yield return StartCoroutine(MoveToPosition(homePosition, duration));
        // 복귀 후 원래 방향으로
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    // 타겟의 방향에 맞춰 스프라이트를 좌우로 뒤집는 함수
    private void FaceTarget(Vector3 targetPosition)
    {
        if (spriteRenderer == null) return;

        float xDirection = targetPosition.x - transform.position.x;
        
        if (xDirection > 0) // 타겟이 오른쪽에 있으면
        {
            // 스프라이트가 기본적으로 오른쪽을 보고 있다고 가정
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (xDirection < 0) // 타겟이 왼쪽에 있으면
        {
            // x축 스케일을 -로 바꿔 좌우 반전
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}