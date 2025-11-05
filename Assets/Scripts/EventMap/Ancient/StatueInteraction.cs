// 파일명: StatueInteraction.cs (최종 수정 버전)
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatueInteraction : MonoBehaviour
{
    [Header("상태 설정")]
    [SerializeField] private bool isUnlocked = false; // 포탈의 현재 활성화 상태

    [Header("이동할 씬 이름")]
    public string sceneToLoad;

    [Header("복귀 지점")]
    public Transform returnSpawnPoint;

    // (static 변수들은 PlayerReturnManager가 사용하므로 그대로 둡니다)
    public static string previousSceneName;
    public static Vector3 returnPosition;
    public static bool hasReturnInfo = false;

    private bool playerIsNear = false;
    private SpriteRenderer spriteRenderer; // 스프라이트를 제어하기 위한 변수
    private Collider2D portalCollider;     // 콜라이더를 제어하기 위한 변수

    private void Awake()
    {
        // 시작하기 전에 필요한 컴포넌트들을 미리 찾아놓습니다.
        spriteRenderer = GetComponent<SpriteRenderer>();
        portalCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        // ★★★ 핵심 로직 1: 시작 시 스스로를 숨깁니다 ★★★
        // 만약 포탈이 아직 활성화되지 않았다면(isUnlocked가 false라면)
        if (!isUnlocked)
        {
            // 자신의 모습을 보이지 않게 하고, 충돌도 감지되지 않게 합니다.
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (portalCollider != null) portalCollider.enabled = false;
        }
    }

    void Update()
    {
        // 활성화된 상태일 때만 'W'키로 입장 가능
        if (playerIsNear && Input.GetKeyDown(KeyCode.W) && isUnlocked)
        {
            EnterPortal();
        }
    }

    // ★★★ 핵심 로직 2: 외부(보스)에서 호출하여 스스로를 드러내는 함수 ★★★
    public void UnlockPortal()
    {
        isUnlocked = true; // 상태를 '활성화'로 변경

        // 다시 모습을 보이게 하고, 충돌도 감지되도록 합니다.
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (portalCollider != null) portalCollider.enabled = true;

        Debug.Log("포탈이 활성화되었습니다!");
        // (여기에 포탈 스프라이트를 주황색으로 바꾸는 코드를 추가해도 좋습니다)
        // 예: spriteRenderer.sprite = unlockedSprite;
    }

    private void EnterPortal()
    {
        // (복귀 정보 저장 및 씬 이동 로직은 기존과 동일)
        previousSceneName = SceneManager.GetActiveScene().name;
        if (returnSpawnPoint != null) returnPosition = returnSpawnPoint.position;
        else returnPosition = transform.position;
        hasReturnInfo = true;
        SceneManager.LoadScene(sceneToLoad);
    }

    // OnTrigger 함수들은 이제 콜라이더가 켜졌을 때만 작동합니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsNear = false;
    }
}