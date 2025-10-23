using TMPro;
using UnityEngine;

/// <summary>
/// 문의 보여지는 부분을 담당하는 클래스
/// </summary>
public class DoorView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _rewardText;

    public void SetReward(RoomInfo roomInfo)
    {
        // 방의 정보에 따라 보상 결정
        if (roomInfo.TryGetTechSelect(out var tech))
        {
            // TechSelect 우선 처리
            _rewardText.text = $"Tech: {tech.ToString()}";
        }
        else if (roomInfo.TryGetNormal(out var normal))
        {
            // Normal 처리
            _rewardText.text = $"Normal: {normal.ToString()}";
        }
        else
        {
            // RoomType 처리
            var roomType = roomInfo.RoomType;
            _rewardText.text = $"Room: {roomType.ToString()}";
        }

    }

    public void ToggleReward(bool isOn)
    {
        _rewardText.gameObject.SetActive(isOn);
    }
}