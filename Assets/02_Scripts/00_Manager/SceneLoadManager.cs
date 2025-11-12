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

        // 맵 미리 생성 후 꺼주기
        GameObject bossRoom = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Map/Rooms/BossRoom", Vector3.zero, Quaternion.identity);
        GameObject NormalRoom = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Map/Rooms/Normal", Vector3.zero, Quaternion.identity);
        GameObject ShopRoom = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Map/Rooms/Shop", Vector3.zero, Quaternion.identity);
        GameObject StartRoom = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Map/Rooms/StartRoom", Vector3.zero, Quaternion.identity);
        GameObject Colosseum = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Map/Rooms/Colosseum", Vector3.zero, Quaternion.identity);
        GameObject Laboratory = GameManager.Instance.PoolManager.GetFromPool("Prefabs/Map/Rooms/Laboratory", Vector3.zero, Quaternion.identity);
        bossRoom.SetActive(false);
        NormalRoom.SetActive(false);
        ShopRoom.SetActive(false);
        StartRoom.SetActive(false);
        Colosseum.SetActive(false);
        Laboratory.SetActive(false);
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
