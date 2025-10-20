using UnityEngine;

/// <summary>
/// 방의 개수별 가중치(생성 확률)를 저장하는 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "RoomCountWeightSO", menuName = "ScriptableObjects/Map/RoomCountWeightSO")]
public class RoomCountWeightSO : ScriptableObject
{
    [SerializeField] float _one = 0.1f;
    [SerializeField] float _two = 0.6f;
    [SerializeField] float _three = 0.3f;

    public float One => _one;
    public float Two => _two;
    public float Three => _three;
}