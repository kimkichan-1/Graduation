using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponChoice : MonoBehaviour
{
    void Start()
    {
        // Weapon 씬에 도착했다는 것은 NewGame 플로우
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isNewGame = true;
        }
    }

    public void SelectWeapon(string weaponName)
    {
        // GameData에 저장 (기존 시스템과의 호환성)
        GameData.SelectedWeapon = weaponName;

        // GameManager에 무기 선택 정보 전달
        if (GameManager.Instance != null)
        {
            GameManager.Instance.selectedWeapon = weaponName;
        }

        // LoadGame 씬으로 이동하여 슬롯 선택
        SceneManager.LoadScene("LoadGame");
    }
}
