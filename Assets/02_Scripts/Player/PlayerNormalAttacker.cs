using UnityEngine;

/// <summary>
/// 플레이어의 일반공격을 구현하는 추상 클래스
/// 무기타입에 따라 상속받아 구현
/// </summary>
public abstract class PlayerNormalAttacker : MonoBehaviour
{
    public abstract void Attack();
}
