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

        // 맵 미리 생성
        GameObject bossRoom = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Map/Rooms/Boss", Vector3.zero, Quaternion.identity);
        GameObject NormalRoom = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Map/Rooms/Normal", Vector3.zero, Quaternion.identity);
        GameObject ShopRoom = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Map/Rooms/Shop", Vector3.zero, Quaternion.identity);
        GameObject StartRoom = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Map/Rooms/Start", Vector3.zero, Quaternion.identity);
        GameObject Colosseum = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Map/Rooms/Colosseum", Vector3.zero, Quaternion.identity);
        GameObject Laboratory = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Map/Rooms/Lab", Vector3.zero, Quaternion.identity);
        
        // 맵 릴리즈
        GameManager.Instance.PoolManager.ReleaseToPool(bossRoom);
        GameManager.Instance.PoolManager.ReleaseToPool(NormalRoom);
        GameManager.Instance.PoolManager.ReleaseToPool(ShopRoom);
        GameManager.Instance.PoolManager.ReleaseToPool(StartRoom);
        GameManager.Instance.PoolManager.ReleaseToPool(Colosseum);
        GameManager.Instance.PoolManager.ReleaseToPool(Laboratory);
    }

    public void LoadPlayScene()
    {
        GameManager.Instance.PlayerStatManager.Initialize();
        GameManager.Instance.PlayerStatManager.UploadToFirebase();
        GameManager.Instance.skillManager.Reset();
        EventBus.ResetEvent();
        GameManager.Instance.CurrencyManager.SetCreditFromServer();
        SceneManager.LoadScene(Constants.SCENE_NAME_PLAY);
    }

    public void LoadLoginScene()
    {
        SceneManager.LoadScene(Constants.SCENE_NAME_LOGIN);
    }
}
