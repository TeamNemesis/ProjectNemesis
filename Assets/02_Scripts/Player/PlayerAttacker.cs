using UnityEngine;

/// <summary>
/// 플레이어의 공격을 구현하는 추상 클래스
/// 무기타입에 따라 상속받아 구현
/// </summary>
public abstract class PlayerAttacker : MonoBehaviour
{
    public abstract void Attack();
}
