// 파일명: ClearSceneManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearSceneManager : MonoBehaviour
{
    // "To Main" 버튼 클릭 시 호출될 함수
    public void GoToMainMenu()
    {
        // 1. (동일) 멈췄던 시간을 1f (정상 속도)로 되돌립니다.
        Time.timeScale = 1f;

       

        // 4. (동일) DontDestroyOnLoadManager의 플래그를 설정합니다.
        DontDestroyOnLoadManager.isReturningToMainMenu = true;

        // 5. (동일) "Main" 씬을 로드합니다.
        SceneManager.LoadScene("Main");
    }
}
