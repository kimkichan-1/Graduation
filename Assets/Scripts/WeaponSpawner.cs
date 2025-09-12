using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public GameObject swordPrefab;
    public GameObject lancePrefab;
    public GameObject macePrefab;

    void Start()
    {
        if (!string.IsNullOrEmpty(GameData.SelectedWeapon))
        {
            GameObject weaponToSpawn = null;
            Vector3 spawnPosition = Vector3.zero;

            switch (GameData.SelectedWeapon)
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
            }
            else
            {
                Debug.LogWarning("Selected weapon prefab not found: " + GameData.SelectedWeapon);
            }
        }
        else
        {
            Debug.Log("No weapon selected.");
        }
    }
}
