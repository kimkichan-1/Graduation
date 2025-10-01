// 파일명: SoulFlame.cs
using UnityEngine;

public class SoulFlame : MonoBehaviour
{
    [Tooltip("불꽃이 생성된 후 사라지기까지 걸리는 시간(초)")]
    public float lifetime = 3.0f; // 3초로 기본값 설정

    void Start()
    {
        // 이 스크립트가 붙어있는 게임 오브젝트(즉, 불꽃 자신)를
        // lifetime 변수에 설정된 시간(초) 후에 파괴하도록 예약합니다.
        Destroy(gameObject, lifetime);
    }
}