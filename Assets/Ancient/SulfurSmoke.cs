// 파일명: SulfurSmoke.cs (최종 완성 버전)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SulfurSmoke : MonoBehaviour
{
    private Animator animator;
    private Collider2D damageCollider;

    // 이 연기에 의해 감속된 플레이어들을 추적하는 리스트
    private List<PlayerController> slowedPlayers = new List<PlayerController>();

    void Awake()
    {
        animator = GetComponent<Animator>();
        damageCollider = GetComponent<Collider2D>();
        if (damageCollider != null)
        {
            damageCollider.enabled = false;
        }
    }

    // Spawner가 호출하여 애니메이션을 시작시키는 함수
    public void ActivateTrap()
    {
        if (animator != null)
        {
            animator.SetTrigger("Activate");
        }
    }

    // --- 애니메이션 이벤트에서 호출할 함수들 ---

    // 1. 폭발 애니메이션에서 호출 (피해 영역 활성화)
    public void EnableDamageZone()
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = true;
        }
    }

    // 2. 폭발 애니메이션 끝날 무렵 호출 (피해 영역 비활성화)
    public void DisableDamageZone()
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = false;
        }
    }

    // 3. 애니메이션이 끝나고 오브젝트를 스스로 파괴
    public void DestroySelf()
    {
        // 파괴되기 직전, 감속시킨 모든 플레이어의 속도를 되돌립니다.
        RestoreAllPlayerSpeeds();
        Destroy(gameObject);
    }

    // --- 플레이어 감속 로직 ---

    // 플레이어가 피해 영역에 들어왔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 피해 영역이 활성화 상태이고, 부딪힌 것이 플레이어일 때만 작동
        if (damageCollider != null && damageCollider.enabled && other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            // 플레이어 스크립트가 있고, 아직 감속되지 않았다면
            if (player != null && !slowedPlayers.Contains(player))
            {
                ApplySmokeSlow(player, 0.5f); // 50% 속도로 감속
            }
        }
    }

    // 플레이어가 피해 영역에서 나갔을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            // 플레이어 스크립트가 있고, 감속된 상태라면
            if (player != null && slowedPlayers.Contains(player))
            {
                RestorePlayerSpeed(player); // 즉시 속도 원상복구
            }
        }
    }

    // 감속 효과를 적용하는 함수
    private void ApplySmokeSlow(PlayerController player, float slowFactor)
    {
        // 중복 적용 방지용 마커가 없을 때만 적용
        if (player.GetComponent<SmokeSlowMarker>() == null)
        {
            player.gameObject.AddComponent<SmokeSlowMarker>();
            player.moveSpeed *= slowFactor; // 현재 속도에 감속 배율을 곱함
            slowedPlayers.Add(player);      // 감속된 플레이어 리스트에 추가하여 추적
        }
    }

    // 특정 플레이어의 속도를 되돌리는 함수
    private void RestorePlayerSpeed(PlayerController player)
    {
        if (player != null)
        {
            var marker = player.GetComponent<SmokeSlowMarker>();
            if (marker != null)
            {
                // ★ 중요: baseMoveSpeed를 사용하여 항상 기본 속도로 복구
                player.moveSpeed = player.baseMoveSpeed;
                Destroy(marker); // 마커 제거
                slowedPlayers.Remove(player); // 리스트에서 제거
            }
        }
    }

    // 이 연기가 파괴될 때, 감속시킨 모든 플레이어의 속도를 되돌리는 함수
    private void RestoreAllPlayerSpeeds()
    {
        // 리스트를 복사하여 순회 (원본 리스트를 직접 변경하며 순회할 때 발생하는 오류 방지)
        List<PlayerController> playersToRestore = new List<PlayerController>(slowedPlayers);
        foreach (var player in playersToRestore)
        {
            RestorePlayerSpeed(player);
        }
        slowedPlayers.Clear(); // 만약을 위해 리스트를 완전히 비움
    }
}

// 감속 효과 중복 적용을 막기 위한 빈 클래스 (이 부분은 수정할 필요 없습니다)
public class SmokeSlowMarker : MonoBehaviour { }
