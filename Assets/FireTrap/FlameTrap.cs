using System.Collections;
using UnityEngine;

public class FlameTrap : MonoBehaviour
{
    // Inspector 창에서 연결할 변수들
    public GameObject explosionEffectPrefab; // 'ExplosionEffect' 프리팹을 연결
    public GameObject damageZone;
    public GameObject warningSign;

    // 타이밍 설정
    public float warningDuration = 2f;
    public float activeDuration = 1f;
    public float inactiveDuration = 3f;

    void Start()
    {
        StartCoroutine(FlameCycleCoroutine());
    }

    IEnumerator FlameCycleCoroutine()
    {
        while (true) // 무한 반복
        {
            // --- 1. 함정이 꺼진 상태로 대기 ---
            yield return new WaitForSeconds(inactiveDuration);

            // --- 2. 예고 표시 활성화 ---
            warningSign.SetActive(true);
            yield return new WaitForSeconds(warningDuration);
            warningSign.SetActive(false);

            // --- 3. 폭발 생성 및 피해 영역 활성화 ---
            // 이 함정의 위치에 'ExplosionEffect' 프리팹을 '복제해서 생성'합니다.
            GameObject explosionInstance = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            damageZone.SetActive(true);

            // 4. 폭발이 지속되는 시간만큼 기다립니다.
            yield return new WaitForSeconds(activeDuration);

            // --- 5. 피해 영역 비활성화 및 폭발 파괴 ---
            damageZone.SetActive(false);
            // 생성했던 폭발 오브젝트를 '파괴'합니다.
            Destroy(explosionInstance);
        }
    }
}

