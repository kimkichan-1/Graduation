using UnityEngine;

public class CameraFallback : MonoBehaviour
{
    void Start()
    {
        // 다른 카메라가 이미 있다면 이 카메라 비활성화
        if (Camera.main != null && Camera.main != GetComponent<Camera>())
            gameObject.SetActive(false);
    }
}
