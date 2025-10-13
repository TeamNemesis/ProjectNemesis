using UnityEngine;

/// <summary>
/// 현재 방에서 다음 방으로 넘어갈 때 맵을 스폰하는 역할을 하는 클래스
/// </summary>
public class MapSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] _doorPrefabs; // 생성할 방 프리팹들
}
