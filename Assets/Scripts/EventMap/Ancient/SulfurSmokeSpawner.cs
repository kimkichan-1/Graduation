// 파일명: SulfurSmokeSpawner.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SulfurSmokeSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject sulfurSmokePrefab;
    public int maxSmokeCount = 2;
    public float minSpawnInterval = 5f;
    public float maxSpawnInterval = 8f;

    [Header("맵 영역")]
    public Transform spawnMin;
    public Transform spawnMax;

    private List<GameObject> activeSmokes = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnSmokeRoutine());
    }

    private IEnumerator SpawnSmokeRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            // 파괴되어 없어진 오브젝트(null)를 리스트에서 제거
            activeSmokes.RemoveAll(item => item == null);

            // 현재 개수가 최대치보다 적을 때만 생성
            if (activeSmokes.Count < maxSmokeCount)
            {
                SpawnSmoke();
            }
        }
    }

    void SpawnSmoke()
    {
        if (sulfurSmokePrefab == null || spawnMin == null || spawnMax == null)
        {
            Debug.LogError("Spawner 설정이 비어있습니다! Prefab과 영역을 지정해주세요.");
            return;
        }

        float spawnX = Random.Range(spawnMin.position.x, spawnMax.position.x);
        float spawnY = Random.Range(spawnMin.position.y, spawnMax.position.y);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);

        GameObject newSmoke = Instantiate(sulfurSmokePrefab, spawnPosition, Quaternion.identity);
        activeSmokes.Add(newSmoke);

        SulfurSmoke smokeScript = newSmoke.GetComponent<SulfurSmoke>();
        if (smokeScript != null)
        {
            smokeScript.ActivateTrap();
        }
    }
}