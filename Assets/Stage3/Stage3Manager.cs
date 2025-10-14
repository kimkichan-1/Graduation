// Stage3_Manager.cs

using UnityEngine;

public class Stage3_Manager : MonoBehaviour
{
    void Start()
    {
        // "Player" 태그를 가진 플레이어 오브젝트를 찾습니다.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("씬에 'Player' 태그를 가진 오브젝트가 없습니다!");
            return;
        }

        // 플레이어에게 PlayerSwimming 컴포넌트가 있는지 확인합니다.
        PlayerSwimming swimmingLogic = player.GetComponent<PlayerSwimming>();
        if (swimmingLogic == null)
        {
            // 없다면, 새로 추가해줍니다.
            swimmingLogic = player.AddComponent<PlayerSwimming>();
        }

        // 헤엄 로직을 활성화시킵니다.
        swimmingLogic.enabled = true;
    }
}