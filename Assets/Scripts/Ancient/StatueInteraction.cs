// ���ϸ�: StatueInteraction.cs (���� ���� ����)
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatueInteraction : MonoBehaviour
{
    [Header("���� ����")]
    [SerializeField] private bool isUnlocked = false; // ��Ż�� ���� Ȱ��ȭ ����

    [Header("�̵��� �� �̸�")]
    public string sceneToLoad;

    [Header("���� ����")]
    public Transform returnSpawnPoint;

    // (static �������� PlayerReturnManager�� ����ϹǷ� �״�� �Ӵϴ�)
    public static string previousSceneName;
    public static Vector3 returnPosition;
    public static bool hasReturnInfo = false;

    private bool playerIsNear = false;
    private SpriteRenderer spriteRenderer; // ��������Ʈ�� �����ϱ� ���� ����
    private Collider2D portalCollider;     // �ݶ��̴��� �����ϱ� ���� ����

    private void Awake()
    {
        // �����ϱ� ���� �ʿ��� ������Ʈ���� �̸� ã�Ƴ����ϴ�.
        spriteRenderer = GetComponent<SpriteRenderer>();
        portalCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        // �ڡڡ� �ٽ� ���� 1: ���� �� �����θ� ����ϴ� �ڡڡ�
        // ���� ��Ż�� ���� Ȱ��ȭ���� �ʾҴٸ�(isUnlocked�� false���)
        if (!isUnlocked)
        {
            // �ڽ��� ����� ������ �ʰ� �ϰ�, �浹�� �������� �ʰ� �մϴ�.
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (portalCollider != null) portalCollider.enabled = false;
        }
    }

    void Update()
    {
        // Ȱ��ȭ�� ������ ���� 'W'Ű�� ���� ����
        if (playerIsNear && Input.GetKeyDown(KeyCode.W) && isUnlocked)
        {
            EnterPortal();
        }
    }

    // �ڡڡ� �ٽ� ���� 2: �ܺ�(����)���� ȣ���Ͽ� �����θ� �巯���� �Լ� �ڡڡ�
    public void UnlockPortal()
    {
        isUnlocked = true; // ���¸� 'Ȱ��ȭ'�� ����

        // �ٽ� ����� ���̰� �ϰ�, �浹�� �����ǵ��� �մϴ�.
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (portalCollider != null) portalCollider.enabled = true;

        Debug.Log("��Ż�� Ȱ��ȭ�Ǿ����ϴ�!");
        // (���⿡ ��Ż ��������Ʈ�� ��Ȳ������ �ٲٴ� �ڵ带 �߰��ص� �����ϴ�)
        // ��: spriteRenderer.sprite = unlockedSprite;
    }

    private void EnterPortal()
    {
        // (���� ���� ���� �� �� �̵� ������ ������ ����)
        previousSceneName = SceneManager.GetActiveScene().name;
        if (returnSpawnPoint != null) returnPosition = returnSpawnPoint.position;
        else returnPosition = transform.position;
        hasReturnInfo = true;
        SceneManager.LoadScene(sceneToLoad);
    }

    // OnTrigger �Լ����� ���� �ݶ��̴��� ������ ���� �۵��մϴ�.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsNear = false;
    }

    /// <summary>
    /// static 변수 초기화 (게임 오버 또는 메인 메뉴 복귀 시 호출)
    /// </summary>
    public static void ResetStaticVariables()
    {
        previousSceneName = null;
        returnPosition = Vector3.zero;
        hasReturnInfo = false;
        Debug.Log("StatueInteraction: static 변수들 초기화됨");
    }
}