// 파일명: PlayerReturnManager.cs
using UnityEngine;

public class PlayerReturnManager : MonoBehaviour
{
    // 씬이 시작될 때 딱 한 번 실행됩니다.
    void Start()
    {
        // StatueInteraction 스크립트에 저장된 복귀 정보가 있는지 확인합니다.
        if (StatueInteraction.hasReturnInfo)
        {
            // "Player" 태그를 가진 오브젝트를 씬에서 찾습니다.
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            // 플레이어가 존재하면, 저장된 위치로 즉시 이동시킵니다.
            if (player != null)
            {
                player.transform.position = StatueInteraction.returnPosition;
            }

            // 사용한 정보는 초기화하여, 다음에 이 씬을 그냥 시작할 때는 적용되지 않도록 합니다.
            StatueInteraction.hasReturnInfo = false;
        }
    }
}
