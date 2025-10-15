using UnityEngine;

/// <summary>
/// 플레이어의 유탄공격을 담당하는 클래스
/// 무기타입에 상관없이 공통으로 사용
/// </summary>
public class PlayerGrenadeAttacker : MonoBehaviour
{
    
    public void GrenadeAttack()
    {
        Debug.Log("유탄 공격 실행");
    }

}