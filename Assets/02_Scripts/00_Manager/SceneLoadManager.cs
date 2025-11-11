using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬을 관리하는 매니저 클래스
/// </summary>
public class SceneLoadManager : MonoBehaviour
{
 
    public void LoadIntroScene()
    {
        GameManager.Instance.PlayerStatManager.Initialize();
        GameManager.Instance.PlayerStatManager.UploadToFirebase();
        GameManager.Instance.skillManager.Reset();
        EventBus.ResetEvent();
        SceneManager.LoadScene(Constants.SCENE_NAME_INTRO);
        GameManager.Instance.PoolManager.ClearAllPools();

    }

    public void LoadPlayScene()
    {
        GameManager.Instance.PlayerStatManager.Initialize();
        GameManager.Instance.PlayerStatManager.UploadToFirebase();
        GameManager.Instance.skillManager.Reset();
        EventBus.ResetEvent();
        SceneManager.LoadScene(Constants.SCENE_NAME_PLAY);
    }

    public void LoadLoginScene()
    {
        SceneManager.LoadScene(Constants.SCENE_NAME_LOGIN);
    }
}
