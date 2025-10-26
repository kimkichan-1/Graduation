using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxEffect;
    [SerializeField] private bool infiniteScroll;
    [SerializeField] private float textureUnitSizeX;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    private Vector3 lastCameraPosition;

    void Start()
    {
        // 게임이 시작될 때 한 번 카메라를 찾아 설정합니다.
        FindAndSetupCamera();
    }

    // ★★★ 이 함수가 핵심적으로 변경되었습니다 ★★★
    // 플레이어를 기준으로 카메라를 찾아오는 함수
    void FindAndSetupCamera()
    {
        // 1. "Player" 태그를 가진 오브젝트를 찾습니다.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 2. 플레이어 또는 그 자식에게서 Camera 컴포넌트를 찾습니다.
            Camera playerCamera = player.GetComponentInChildren<Camera>();
            if (playerCamera != null)
            {
                // 3. 찾았다면, 이 카메라를 따라다니도록 설정합니다.
                cameraTransform = playerCamera.transform;
            }
        }

        // 4. 만약 플레이어에게서 카메라를 못 찾았다면, 예전처럼 Camera.main을 찾습니다 (안전장치).
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // 5. 최종적으로 카메라를 찾았다면, 초기 위치를 설정합니다.
        if (cameraTransform != null)
        {
            // --- ▼▼▼▼▼ 이 줄을 추가하세요 ▼▼▼▼▼ ---
            // 배경의 X축 위치를 즉시 카메라의 X축 위치로 설정합니다.
            transform.position = new Vector3(cameraTransform.position.x, transform.position.y, transform.position.z);
            // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---
            lastCameraPosition = new Vector3(Mathf.Clamp(cameraTransform.position.x, minX, maxX), cameraTransform.position.y, cameraTransform.position.z);

            if (infiniteScroll && textureUnitSizeX == 0)
            {
                Sprite sprite = GetComponent<SpriteRenderer>().sprite;
                if (sprite != null)
                {
                    Texture2D texture = sprite.texture;
                    textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
                }
            }
        }
        else
        {
            // 어떤 카메라도 찾지 못했다면 경고를 띄웁니다.
            Debug.LogWarning("ParallaxBackground: 플레이어의 카메라 또는 메인 카메라를 찾을 수 없습니다.", this.gameObject);
        }
    }

    void LateUpdate()
    {
        // 매 프레임마다 카메라 참조가 유효한지(파괴되지 않았는지) 확인합니다.
        if (cameraTransform == null)
        {
            // 참조가 유실되었다면 (씬 전환 등), 다시 찾아봅니다.
            FindAndSetupCamera();

            // 그래도 찾지 못했다면 이번 프레임은 그냥 넘어갑니다.
            if (cameraTransform == null)
            {
                return;
            }
        }

        float clampedCameraX = Mathf.Clamp(cameraTransform.position.x, minX, maxX);
        Vector3 currentCameraPosition = new Vector3(clampedCameraX, cameraTransform.position.y, cameraTransform.position.z);

        Vector3 deltaMovement = currentCameraPosition - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffect, 0, 0);
        lastCameraPosition = currentCameraPosition;

        if (infiniteScroll)
        {
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(cameraTransform.position.x - offsetPositionX, transform.position.y, transform.position.z);
            }
        }
    }
}