// 파일명: SeaMonsterDeathReporter.cs
using UnityEngine;

[RequireComponent(typeof(MonsterHealth))] // MonsterHealth가 반드시 있어야 함
public class SeaMonsterDeathReporter : MonoBehaviour
{
    private MonsterHealth monsterHealth;
    private bool hasReportedDeath = false; // 보고를 한 번만 하도록 하는 스위치

    void Start()
    {
        // 자신의 MonsterHealth 컴포넌트를 미리 찾아둡니다.
        monsterHealth = GetComponent<MonsterHealth>();
    }

    void Update()
    {
        // 아직 죽음을 보고하지 않았고, 몬스터의 체력 스크립트가 '사망' 상태를 나타내면
        // (monsterHealth.IsDead 프로퍼티는 public이어야 합니다.)
        if (!hasReportedDeath && monsterHealth.IsDead)
        {
            // Stage3Manager가 씬에 존재한다면 보고합니다.
            if (Stage3Manager.instance != null)
            {
                Stage3Manager.instance.ReportSeaMonsterDeath();
            }
            // 다시는 보고하지 않도록 스위치를 켭니다.
            hasReportedDeath = true;
        }
    }
}
