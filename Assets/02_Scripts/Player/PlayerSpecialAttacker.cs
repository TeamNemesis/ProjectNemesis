using UnityEngine;

/// <summary>
/// 플레이어의 특수공격을 담당하는 추상 클래스
/// 무기타입에 따라 상속받아 구현
/// </summary>
public abstract class PlayerSpecialAttacker : MonoBehaviour
{
    public abstract void SpecialAttack();
}