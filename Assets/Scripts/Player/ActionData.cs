using UnityEngine;

// 플레이어와 보스가 사용할 행동 데이터를 정의하는 ScriptableObject
[CreateAssetMenu(fileName = "New Action Data", menuName = "Turn-Based/Action Data")]
public class ActionData : ScriptableObject
{
    public enum ActionType
    {
        Attack,
        Dash,
        Move
        // 필요한 다른 행동 타입 추가 가능
    }

    public string actionName;
    public ActionType type;
    public float value; // 공격력, 이동 거리 등
    public float duration; // 행동 지속 시간 (애니메이션 시간 등)
}