using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public GameObject swordPrefab;
    public GameObject lancePrefab;
    public GameObject macePrefab;

    void Start()
    {
        // GameManager가 데이터를 복원할 때까지 약간 대기 (0.6초 = ApplyDataToPlayer의 0.5초 + 여유)
        Invoke(nameof(SpawnWeapon), 0.6f);
    }

    private void SpawnWeapon()
    {
        // GameData.SelectedWeapon 확인 (NewGame 플로우)
        string weaponToSpawnName = GameData.SelectedWeapon;

        // GameManager에 저장된 무기 정보도 확인 (LoadGame 플로우)
        if (string.IsNullOrEmpty(weaponToSpawnName) && GameManager.Instance != null)
        {
            weaponToSpawnName = GameManager.Instance.selectedWeapon;
        }

        if (!string.IsNullOrEmpty(weaponToSpawnName) && weaponToSpawnName != "None")
        {
            GameObject weaponToSpawn = null;
            Vector3 spawnPosition = Vector3.zero;

            switch (weaponToSpawnName)
            {
                case "Sword":
                    weaponToSpawn = swordPrefab;
                    spawnPosition = new Vector3(-3.248f, 1.66f, 0);
                    break;
                case "Lance":
                    weaponToSpawn = lancePrefab;
                    spawnPosition = new Vector3(-3.236f, 1.37f, 0);
                    break;
                case "Mace":
                    weaponToSpawn = macePrefab;
                    spawnPosition = new Vector3(-3.227f, 1.615f, 0);
                    break;
            }

            if (weaponToSpawn != null)
            {
                Instantiate(weaponToSpawn, spawnPosition, Quaternion.identity);
                Debug.Log($"{weaponToSpawnName} 무기 스폰 완료");
            }
            else
            {
                Debug.LogWarning($"무기 프리팹을 찾을 수 없습니다: {weaponToSpawnName}");
            }
        }
    }
}
