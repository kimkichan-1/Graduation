// 파일명: MidBossController.cs (수정 버전)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MidBossController : MonoBehaviour
{
    private static List<string> completedEventIDs = new List<string>();
    public string eventID = "DefeatedMidBoss";

    [Header("이벤트 연동")]
    // ★★★ 수정: GameObject 대신 StatueInteraction 스크립트를 직접 연결합니다 ★★★
    [Tooltip("처치 후 활성화시킬 포탈 오브젝트")]
    public StatueInteraction targetPortal;
    [Tooltip("보스 처치 후 나타날 대화 내용")]
    [TextArea(3, 10)]
    public string[] deathDialogue;

    public void TriggerDeathEvent()
    {
        if (completedEventIDs.Contains(eventID)) return;
        completedEventIDs.Add(eventID);

        // ★★★ 수정: SetActive(true) 대신 UnlockPortal() 함수를 호출합니다 ★★★
        if (targetPortal != null)
        {
            Debug.Log("중간 보스 처치! 포탈을 활성화합니다.");
            targetPortal.UnlockPortal(); // 포탈의 상태를 '활성화'로 변경
        }
        else
        {
            Debug.LogError("MidBossController에 활성화할 포탈이 연결되지 않았습니다!");
        }

        if (DialogueController.Instance != null && deathDialogue.Length > 0)
        {
            DialogueController.Instance.StartDialogue(deathDialogue, null);
        }
    }
}
