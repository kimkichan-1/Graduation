// 파일명: FlameTrapSpawner.cs (최종 수정본)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlameTrapSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [Tooltip("생성할 불꽃 트랩 프리팹")]
    public GameObject flameTrapPrefab;
    [Tooltip("동시에 존재할 최대 트랩 개수")]
    public int maxTrapCount = 3;
    [Tooltip("새로운 트랩을 생성하기까지 걸리는 최소 시간")]
    public float minSpawnInterval = 3f;
    [Tooltip("새로운 트랩을 생성하기까지 걸리는 최대 시간")]
    public float maxSpawnInterval = 6f;


    [Header("맵 영역")]
    [Tooltip("스폰 가능한 최소 좌표 (왼쪽 아래)")]
    public Transform spawnMin;
    [Tooltip("스폰 가능한 최대 좌표 (오른쪽 위)")]
    public Transform spawnMax;

    // 현재 맵에 활성화된 트랩들을 관리하는 리스트
    private List<GameObject> activeTraps = new List<GameObject>();

    void Start()
    {
        // 스폰을 관리하는 코루틴 시작
        StartCoroutine(SpawnTrapRoutine());
    }

    private IEnumerator SpawnTrapRoutine()
    {
        while (true)
        {
            // 다음 스폰까지 랜덤한 시간만큼 대기
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            // 리스트에 있는 오브젝트 중 파괴된 것(null)이 있으면 정리
            activeTraps.RemoveAll(item => item == null);

            // 현재 생성된 함정 개수가 최대 개수보다 적을 때만 새로 생성
            if (activeTraps.Count < maxTrapCount)
            {
                SpawnTrap();
            }
        }
    }

    void SpawnTrap()
    {
        if (flameTrapPrefab == null || spawnMin == null || spawnMax == null)
        {
            Debug.LogWarning("스포너 설정이 완료되지 않았습니다.");
            return;
        }

        // 스폰 영역 내에서 랜덤한 위치 계산
        float spawnX = Random.Range(spawnMin.position.x, spawnMax.position.x);
        float spawnY = Random.Range(spawnMin.position.y, spawnMax.position.y);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);

        // 함정 생성 및 리스트에 추가
        GameObject newTrap = Instantiate(flameTrapPrefab, spawnPosition, Quaternion.identity);
        activeTraps.Add(newTrap);
    }
}