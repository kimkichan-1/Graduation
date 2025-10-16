// 파일명: UnifiedFlameTrap.cs (최종 수정본)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnifiedFlameTrap : MonoBehaviour
{
    [Header("구성 요소")]
    [SerializeField] private GameObject warningSign;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private Collider2D damageCollider;

    [Header("타이밍 설정")]
    [SerializeField] private float warningDuration = 2f;
    [SerializeField] private float activeDuration = 0.5f;
    [SerializeField] private float cooldownDuration = 3f;

    [Header("패널티 설정")]
    [SerializeField] private int penaltyAmount = 2;

    private bool isExplosionActive = false;
    // 이번 폭발 주기 동안 이미 피해를 입은 '플레이어 게임 오브젝트'를 저장하는 리스트
    private List<GameObject> hitPlayersThisCycle = new List<GameObject>();

    void Start()
    {
        if (warningSign != null) warningSign.SetActive(false);
        if (explosionEffect != null) explosionEffect.SetActive(false);

        if (damageCollider == null)
        {
            damageCollider = GetComponentInChildren<Collider2D>();
        }

        StartCoroutine(TrapCycleCoroutine());
    }

    private IEnumerator TrapCycleCoroutine()
    {
        while (true)
        {
            // --- 1. 대기 상태 ---
            isExplosionActive = false;
            yield return new WaitForSeconds(cooldownDuration);

            // --- 2. 전조증상 ---
            if (warningSign != null) warningSign.SetActive(true);
            yield return new WaitForSeconds(warningDuration);
            if (warningSign != null) warningSign.SetActive(false);

            // --- 3. 폭발 시작 ---
            if (explosionEffect != null) explosionEffect.SetActive(true);

            isExplosionActive = true;
            hitPlayersThisCycle.Clear(); // 새 폭발이 시작됐으니, 맞은 플레이어 목록 초기화

            // 폭발이 지속되는 시간만큼 기다림
            yield return new WaitForSeconds(activeDuration);
            if (explosionEffect != null) explosionEffect.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isExplosionActive || !other.CompareTag("Player"))
        {
            return;
        }

        // 충돌한 콜라이더의 '게임 오브젝트'를 가져옴
        GameObject playerObject = other.gameObject;

        // 만약 이번 주기에 아직 맞은 적이 없는 '플레이어'라면
        if (!hitPlayersThisCycle.Contains(playerObject))
        {
            // 1. 점수 차감
            if (BlacksmithMinigameManager.Instance != null)
            {
                BlacksmithMinigameManager.Instance.DecreaseScore(penaltyAmount);
            }

            // 2. 피격 모션 호출
            PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(0);
            }

            // 맞은 플레이어 목록에 '게임 오브젝트'를 추가하여, 이번 주기에는 더 이상 맞지 않도록 함
            hitPlayersThisCycle.Add(playerObject);
        }
    }
}
