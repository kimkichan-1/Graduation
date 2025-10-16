// 파일명: PortalReturnManager.cs
using UnityEngine;

public class PortalReturnManager : MonoBehaviour
{
    void Start()
    {
        // PortalReturnData에 저장된 복귀 정보가 있는지 확인합니다.
        if (PortalReturnData.hasReturnInfo)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // PortalReturnData에 저장된 위치로 플레이어를 이동시킵니다.
                player.transform.position = PortalReturnData.returnPosition;
            }

            // 사용한 정보는 초기화합니다.
            PortalReturnData.hasReturnInfo = false;
        }
    }
}