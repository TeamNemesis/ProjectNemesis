using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬을 관리하는 매니저 클래스
/// </summary>
public class SceneLoadManager : MonoBehaviour
{
 
    public void LoadIntroScene()
    {
        SceneManager.LoadScene(Constants.SCENE_NAME_INTRO);

    }

    public void LoadPlayScene()
    {
        SceneManager.LoadScene(Constants.SCENE_NAME_PLAY);
    }
}
