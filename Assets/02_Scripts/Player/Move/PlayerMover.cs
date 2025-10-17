using UnityEngine;

/// <summary>
/// 플레이어의 이동을 담당하는 클래스
/// </summary>
public class PlayerMover : MonoBehaviour
{
    [SerializeField] CharacterController _characterController;  // 캐릭터 컨트롤러 컴포넌트 참조
    [SerializeField] float _moveSpeed = 5f;                    // 이동 속도

    Vector3 _moveDir; // 이동 방향 벡터

    /// <summary>
    /// 이동 입력을 받았을 때 호출되어 캐릭터 컨트롤러를 통해 플레이어를 움직이는 함수
    /// </summary>
    /// <param name="moveDir"></param>
    public void Move(Vector2 moveDir)
    {
        // 입력된 2D 벡터를 3D 벡터로 변환 (y축은 0으로 고정)
        _moveDir = new Vector3(moveDir.x, 0, moveDir.y).normalized;

        // 이동 방향에 속도를 곱하여 최종 이동 벡터 계산
        _characterController.Move(_moveDir * _moveSpeed * Time.deltaTime);

        // 플레이어가 이동 중이라면 회전 처리
        if (_moveDir != Vector3.zero)
        {
            // 이동 방향으로 플레이어 회전 (부드럽게 회전)
            Quaternion targetRotation = Quaternion.LookRotation(_moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
        }
    }

    /// <summary>
    /// 플레이어의 이동 속도를 설정하는 함수
    /// </summary>
    /// <param name="newSpeed"></param>
    public void SetMoveSpeed(float newSpeed)
    {
        _moveSpeed = newSpeed;
    }
}