using UnityEngine;

public enum NormalDoorType
{
    Credit, // 크레딧 문
    Heal, // 회복 문
    Upgrade, // 업그레이드 문
    Chrome, // 크롬 문
    SkillPack // 스킬팩 문
}

/// <summary>
/// 
/// </summary>
public class NormalDoor : Door
{
    [SerializeField] string door_Name = "normalDoor";
    [SerializeField] float door_Chance = 0.7f; // 노말룸의 등장 확률 설정
    [SerializeField] DoorType door_Type = DoorType.Normal;

    [SerializeField] float door_CreditChance; // 크레딧 방 등장 확률
    [SerializeField] float door_HealChance; // 회복 방 등장 확률
    [SerializeField] float door_UpgradeChance; // 업그레이드 방 등장 확률
    [SerializeField] float door_ChromeChance; // 크롬 방 등장 확률
    [SerializeField] float door_SkillPackChance; // 스킬팩 방 등장 확률

    SkillBase _selectedSkillCompany;    // 이번에 뽑힌 스킬팩

    public override string DoorName => door_Name;
    public override float DoorChance => door_Chance;
    public override DoorType DoorType => door_Type;

    public override void Initialize()
    {
        //Debug.Log("노말룸 생성됨");
        GetRandomDoorType();
    }

    [ContextMenu("임의값 반영하기")]
    void SetDefaultChances()
    {
        door_CreditChance = 0.1f;
        door_HealChance = 0.05f;
        door_UpgradeChance = 0.2f;
        door_ChromeChance = 0.05f;
        door_SkillPackChance = 0.5f;
    }

    public void SetSkillCompany()
    {
        _selectedSkillCompany= GameManager.Instance.skillManager.DrawSkillCompany();
        Debug.Log(_selectedSkillCompany.skillBaseString);
    }

    void SetDoor(int doorType)
    {
        // doorType에 따라 문 생성
        // 예: if (doorType == (int)NormalDoorType.Credit) { /* 크레딧 방 문 생성 코드 */ }
    }

    int GetRandomDoorType()
    {
        float totalChance = door_CreditChance + door_HealChance + door_UpgradeChance + door_ChromeChance + door_SkillPackChance;
        float randomValue = Random.Range(0f, totalChance);
        if (randomValue < door_CreditChance)
        {
            Debug.Log("크레딧방 생성됨");
            return (int)NormalDoorType.Credit;
        }
        else if (randomValue < door_CreditChance + door_HealChance)
        {
            Debug.Log("회복방 생성됨");
            return (int)NormalDoorType.Heal;
        }
        else if (randomValue < door_CreditChance + door_HealChance + door_UpgradeChance)
        {
            Debug.Log("업그레이드방 생성됨");
            return (int)NormalDoorType.Upgrade;
        }
        else if (randomValue < door_CreditChance + door_HealChance + door_UpgradeChance + door_ChromeChance)
        {
            Debug.Log("크롬방 생성됨");
            return (int)NormalDoorType.Chrome;
        }
        else
        {
            Debug.Log("스킬팩방 생성됨");
            return (int)NormalDoorType.SkillPack;
        }
    }
}