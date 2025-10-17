using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main 씬의 메뉴 버튼 관리
/// NewGame/LoadGame 버튼 처리
/// </summary>
public class MainMenuController : MonoBehaviour
{
    /// <summary>
    /// New Game 버튼 - Weapon 씬으로 이동
    /// </summary>
    public void OnNewGameButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PrepareNewGame();
        }

        SceneManager.LoadScene("Weapon");
    }

    /// <summary>
    /// Load Game 버튼 - LoadGame 씬으로 이동
    /// </summary>
    public void OnLoadGameButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isNewGame = false;
        }

        Debug.Log("Load Game - LoadGame 씬으로 이동");
        SceneManager.LoadScene("LoadGame");
    }

    /// <summary>
    /// How To Play 버튼
    /// </summary>
    public void OnHowToPlayButton()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    /// <summary>
    /// Settings 버튼
    /// </summary>
    public void OnSettingsButton()
    {
        SceneManager.LoadScene("Setting");
    }

    /// <summary>
    /// Quit 버튼
    /// </summary>
    public void OnQuitButton()
    {
        Debug.Log("게임 종료");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
