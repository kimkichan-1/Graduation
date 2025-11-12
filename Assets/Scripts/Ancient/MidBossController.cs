// 파일명: MidBossController.cs (수정됨)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MidBossController : MonoBehaviour
{
    // static 리스트는 게임 세션 동안 유지됩니다.
    public static List<string> completedEventIDs = new List<string>();
    [Tooltip("이 보스 이벤트의 고유 ID")]
    public string eventID = "DefeatedMidBoss";

    [Header("이벤트 연동")]
    [Tooltip("처치 후 활성화시킬 석상/포탈 오브젝트")]
    public StatueInteraction targetPortal; // ★★★ 중요: 이 변수가 실제로 사용되고 있는지 확인 필요 ★★★
    [Tooltip("보스 처치 후 나타날 대화 내용")]
    [TextArea(3, 10)]
    public string[] deathDialogue;

    [Header("사망 후 이벤트 추가")]
    [Tooltip("처치 후 비활성화할 벽 게임 오브젝트")]
    [SerializeField] private GameObject wallToDisable;
    [Tooltip("처치 후 활성화할 화살표 (부모) 게임 오브젝트")]
    [SerializeField] private GameObject arrowsToEnable;

    void Start()
    {
        // --- ▼▼▼▼▼ 수정된 부분 (벽/화살표 초기 상태 설정 및 보스 비활성화) ▼▼▼▼▼ ---

        // 1. 벽과 화살표의 초기 상태를 먼저 설정합니다. (벽 활성화, 화살표 비활성화)
        //    씬을 다시 로드할 때마다 이 상태로 시작하게 됩니다.
        if (wallToDisable != null) wallToDisable.SetActive(true);
        if (arrowsToEnable != null) arrowsToEnable.SetActive(false);

        // 2. 이 보스가 이전에 이미 처치되었는지 확인합니다.
        if (completedEventIDs.Contains(eventID))
        {
            // 이미 완료된 보스라면, 보스 게임 오브젝트만 비활성화합니다.
            // 벽과 화살표는 위에서 설정한 초기 상태를 유지합니다.
            Debug.Log($"보스 이벤트 '{eventID}'는 이미 완료되었습니다. 보스만 비활성화합니다.");
            gameObject.SetActive(false);
        }
        // --- ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ ---
    }

    // 이 함수는 MonsterHealth의 DieRoutine에서 호출됩니다.
    public void TriggerDeathEvent()
    {
        // 이벤트가 이미 완료되었다면 실행하지 않음
        if (completedEventIDs.Contains(eventID)) return;
        // 완료 목록에 이 보스의 ID를 추가하여 '기억'시킵니다.
        completedEventIDs.Add(eventID);

        // --- 기존 코드 (포탈 활성화 - 필요하다면 유지) ---
        if (targetPortal != null)
        {
            Debug.Log("중간 보스 처치! 포탈을 활성화합니다.");
            targetPortal.UnlockPortal();
        }
        // ---

        // --- 기존 코드 (벽 비활성화 & 화살표 활성화) ---
        // 이제 이 함수가 호출될 때만 벽과 화살표 상태가 변경됩니다.
        if (wallToDisable != null)
        {
            wallToDisable.SetActive(false);
            Debug.Log(wallToDisable.name + " 벽이 비활성화되었습니다.");
        }
        if (arrowsToEnable != null)
        {
            arrowsToEnable.SetActive(true);
            Debug.Log(arrowsToEnable.name + " 화살표가 활성화되었습니다.");
        }
        // ---

        // --- 기존 코드 (사망 대화 출력) ---
        if (DialogueController.Instance != null && deathDialogue.Length > 0)
        {
            DialogueController.Instance.StartDialogue(deathDialogue, null);
        }
        // ---
    }

    /// <summary>
    /// static 변수 초기화 (게임 오버 또는 메인 메뉴 복귀 시 호출)
    /// </summary>
    public static void ResetStaticVariables()
    {
        completedEventIDs.Clear();
        Debug.Log("MidBossController: completedEventIDs 초기화됨");
    }
}
