using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponChoice : MonoBehaviour
{
    public void SelectWeapon(string weaponName)
    {
        GameData.SelectedWeapon = weaponName;
        SceneManager.LoadScene("Stage1");
    }
}
