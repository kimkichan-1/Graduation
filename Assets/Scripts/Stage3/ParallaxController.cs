using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [Tooltip("배경들이 따라다닐 메인 카메라")]
    [SerializeField] private Transform cameraTransform;

    [Tooltip("배경 레이어와 각 레이어의 이동 속도")]
    [SerializeField] private ParallaxLayer[] parallaxLayers;

    private Vector3 cameraStartPosition;

    // [System.Serializable]을 사용하면 Inspector 창에서 이 클래스의 변수들을 설정할 수 있습니다.
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform background;
        [Range(0f, 1f)]
        public float parallaxMultiplier;
        [HideInInspector] public Vector3 startPosition;
    }

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // 게임 시작 시 카메라와 모든 배경 레이어의 초기 위치를 저장합니다.
        cameraStartPosition = cameraTransform.position;
        foreach (var layer in parallaxLayers)
        {
            layer.startPosition = layer.background.position;
        }
    }

    // LateUpdate는 모든 Update 함수가 실행된 후 마지막에 호출됩니다.
    // 카메라의 최종 위치를 기준으로 배경을 움직이기에 가장 적합합니다.
    void LateUpdate()
    {
        // 카메라가 시작 위치에서 얼마나 움직였는지 계산합니다.
        Vector3 distanceMoved = cameraTransform.position - cameraStartPosition;

        // 각 배경 레이어를 'parallaxMultiplier' 값에 따라 다르게 움직입니다.
        foreach (var layer in parallaxLayers)
        {
            float parallaxMoveX = distanceMoved.x * layer.parallaxMultiplier;
            Vector3 newPosition = new Vector3(layer.startPosition.x + parallaxMoveX, layer.background.position.y, layer.background.position.z);
            layer.background.position = newPosition;
        }
    }
}
