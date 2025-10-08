// CameraController.cs

using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("타겟 및 설정")]
    public Transform playerTransform; // 카메라가 따라갈 플레이어
    public float zoomOrthographicSize = 3f; // 확대했을 때의 카메라 크기 (작을수록 확대)
    public float zoomDuration = 0.5f; // 확대/축소에 걸리는 시간

    private Camera cam;
    private float originalOrthographicSize;
    private Vector3 originalPosition;

    void Start()
    {
        cam = Camera.main;
        // 게임 시작 시 원래의 카메라 크기와 위치를 저장해 둡니다.
        originalOrthographicSize = cam.orthographicSize;
        originalPosition = transform.position;
    }

    // 플레이어를 중심으로 확대하는 코루틴
    public IEnumerator ZoomIn()
    {
        Debug.Log("카메라 줌 인!");
        // 목표 위치는 플레이어의 위치를 기준으로 하되, 카메라의 Z축 위치는 유지합니다.
        Vector3 targetPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, originalPosition.z);
        
        yield return ZoomAndMove(originalOrthographicSize, zoomOrthographicSize, originalPosition, targetPosition, zoomDuration);
    }

    // 원래 상태로 축소하며 돌아오는 코루틴
    public IEnumerator ZoomOut()
    {
        Debug.Log("카메라 줌 아웃!");
        yield return ZoomAndMove(zoomOrthographicSize, originalOrthographicSize, transform.position, originalPosition, zoomDuration);
    }

    // 실제로 줌과 이동을 부드럽게 처리하는 코루틴
    private IEnumerator ZoomAndMove(float startSize, float endSize, Vector3 startPos, Vector3 endPos, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            // Lerp 함수를 사용하여 현재 값에서 목표 값으로 부드럽게 변경
            cam.orthographicSize = Mathf.Lerp(startSize, endSize, time / duration);
            transform.position = Vector3.Lerp(startPos, endPos, time / duration);
            
            time += Time.deltaTime;
            yield return null;
        }

        // 정확한 값으로 보정
        cam.orthographicSize = endSize;
        transform.position = endPos;
    }
}