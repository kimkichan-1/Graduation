using UnityEngine;

[System.Serializable]
public class WeaponStats
{
    public string weaponName;     // 무기 이름
    public int attackPower;       // 공격력
    public int defense;           // 방어력
    public int bonusHealth;       // 추가 체력
    public float moveSpeed;       // 이동 속도
    public float dashForce;       // 대시 힘
    public float dashDuration;    // 대시 지속 시간
    public float dashCooldown;    // 대시 쿨타임
}
