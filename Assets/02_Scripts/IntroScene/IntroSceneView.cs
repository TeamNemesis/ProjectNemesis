using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인트로 씬의 UI를 관리하는 클래스
/// </summary>
public class IntroSceneView : MonoBehaviour
{
    [SerializeField] Button _gameStartButton;

    public void Initialize()
    {
        SceneLoadManager sceneLoadManager = GameManager.Instance.SceneLoadManager;
        _gameStartButton.onClick.AddListener(sceneLoadManager.LoadPlayScene);
    }
}