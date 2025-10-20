using UnityEngine;

/// <summary>
/// 일반 방 타입별 가중치(생성 확률)를 저장하는 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "NormalRoomTypeWeightSO", menuName = "ScriptableObjects/Map/NormalRoomTypeWeightSO")]
public class NormalRoomTypeWeightSO:ScriptableObject
{
    [SerializeField] float _credit = 0.1f;
    [SerializeField] float _heal = 0.1f;
    [SerializeField] float _chrome = 0.1f;
    [SerializeField] float _techPack = 0.5f;
    [SerializeField] float _techUpgrade = 0.2f;

    public float Credit => _credit;
    public float Heal => _heal;
    public float Chrome => _chrome;
    public float TechPack => _techPack;
    public float TechUpgrade => _techUpgrade;
}