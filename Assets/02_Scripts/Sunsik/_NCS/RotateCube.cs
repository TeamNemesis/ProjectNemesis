using TMPro;
using UnityEngine;

/// <summary>
/// 큐브를 회전시키는 클래스
/// </summary>
public class RotateCube : MonoBehaviour
{
    [SerializeField] InputTest _inputTest; // InputTest 컴포넌트
    [SerializeField] InputButton[] _inputButtons; // InputButton 배열

    [SerializeField] GameObject _cube; // 회전시킬 큐브 오브젝트
    [SerializeField] float _rotateSpeed; // 회전 속도
    [SerializeField] TextMeshProUGUI _isWorldText; // 월드 좌표계 기준 회전 여부 텍스트

    [SerializeField] bool _isWorld = true; // 월드 좌표계 기준 회전 여부

    private void Start()
    {
        _inputTest.OnMoveInput += Rotate; // InputTest의 OnMoveInput 이벤트에 Rotate 함수 등록
        
        foreach (var button in _inputButtons) // 모든 InputButton에 대해
        {
            button.OnButtonClicked += (buttonType) => // 버튼 클릭 시
            {
                Vector3 direction = buttonType switch // 버튼 타입에 따라 방향 설정
                {
                    ButtonType.Left => Vector3.left,
                    ButtonType.Right => Vector3.right,
                    ButtonType.Up => Vector3.up,
                    ButtonType.Down => Vector3.down,
                    _ => Vector3.zero
                };
                Rotate(direction); // 해당 방향으로 큐브 회전
            };
        }

        _isWorld = true;
        _isWorldText.text = _isWorld ? "현재 : 월드 좌표계 기준 회전" : "현재 : 로컬 좌표계 기준 회전"; // 초기 텍스트 설정
    }

    /// <summary>
    /// 입력받은 방향으로 큐브를 회전시키는 함수
    /// </summary>
    /// <param name="direction">입력받은 방향</param>
    public void Rotate(Vector3 direction)
    {
        Vector3 newDirection = new Vector3(direction.y, -direction.x, 0); // 입력 방향을 회전 방향으로 변환

        if (!_isWorld) // 월드 좌표계 기준 회전 여부에 따라

        {
            _cube.transform.Rotate(newDirection * _rotateSpeed * Time.deltaTime); // 방향과 속도에 따라 큐브 회전
        }

        else
        {
            _cube.transform.Rotate(newDirection * _rotateSpeed * Time.deltaTime, Space.World); // 방향과 속도에 따라 큐브 회전
        }
    }

    public void Reset()
    {
        _cube.transform.rotation = Quaternion.identity; // 큐브 회전 초기화
    }

    public void ToggleIsWorld()
    {
        _isWorld = !_isWorld; // 월드 좌표계 기준 회전 여부 토글
        _isWorldText.text = _isWorld ? "현재 : 월드 좌표계 기준 회전" : "현재 : 로컬 좌표계 기준 회전"; // 텍스트 업데이트
    }
}
