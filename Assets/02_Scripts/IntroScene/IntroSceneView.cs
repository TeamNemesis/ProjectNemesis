using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인트로 씬의 UI를 관리하는 클래스
/// </summary>
public class IntroSceneView : MonoBehaviour
{
    [SerializeField] Button _gameStartButton;

    public void LoadPlayScene()
    {
        GameManager.Instance.SceneLoadManager.LoadPlayScene();
    }
}