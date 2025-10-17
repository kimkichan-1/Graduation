// ���ϸ�: MidBossController.cs (���� ����)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MidBossController : MonoBehaviour
{
    public static List<string> completedEventIDs = new List<string>();
    public string eventID = "DefeatedMidBoss";

    [Header("�̺�Ʈ ����")]
    // �ڡڡ� ����: GameObject ��� StatueInteraction ��ũ��Ʈ�� ���� �����մϴ� �ڡڡ�
    [Tooltip("óġ �� Ȱ��ȭ��ų ��Ż ������Ʈ")]
    public StatueInteraction targetPortal;
    [Tooltip("���� óġ �� ��Ÿ�� ��ȭ ����")]
    [TextArea(3, 10)]
    public string[] deathDialogue;

    public void TriggerDeathEvent()
    {
        if (completedEventIDs.Contains(eventID)) return;
        completedEventIDs.Add(eventID);

        // �ڡڡ� ����: SetActive(true) ��� UnlockPortal() �Լ��� ȣ���մϴ� �ڡڡ�
        if (targetPortal != null)
        {
            Debug.Log("�߰� ���� óġ! ��Ż�� Ȱ��ȭ�մϴ�.");
            targetPortal.UnlockPortal(); // ��Ż�� ���¸� 'Ȱ��ȭ'�� ����
        }
        else
        {
            Debug.LogError("MidBossController�� Ȱ��ȭ�� ��Ż�� ������� �ʾҽ��ϴ�!");
        }

        if (DialogueController.Instance != null && deathDialogue.Length > 0)
        {
            DialogueController.Instance.StartDialogue(deathDialogue, null);
        }
    }
}
