// CharacterVisuals.cs (수정된 버전)

using System.Collections;
using UnityEngine;

public class CharacterVisuals : MonoBehaviour
{
    private Vector3 homePosition;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        homePosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        // 이동 함수 안에서 자동으로 방향을 바꾸는 로직을 삭제합니다.
        // FaceTarget(targetPosition); // <- 이 줄 삭제

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
        // 여기서도 방향 전환 로직을 삭제합니다.
        // FaceTarget(homePosition); // <- 이 줄 삭제
        yield return StartCoroutine(MoveToPosition(homePosition, duration));
    }

    // ★★★ 함수 이름과 로직 수정 ★★★
    // 이제 이 함수는 명확히 '상대방'의 위치를 받아 그쪽을 바라봅니다.
    public void FaceOpponent(Transform opponentTransform)
    {
        if (spriteRenderer == null) return;

        float xDirection = opponentTransform.position.x - transform.position.x;
        
        if (xDirection > 0) // 상대가 오른쪽에 있으면 오른쪽을 바라봄
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (xDirection < 0) // 상대가 왼쪽에 있으면 왼쪽을 바라봄
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}