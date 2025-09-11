using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoScene : MonoBehaviour
{
    // UI Button의 OnClick() 이벤트에서 이 함수를 호출할 수 있습니다.
    // Unity 에디터에서 이동할 Scene의 이름을 인자로 전달해야 합니다.
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
