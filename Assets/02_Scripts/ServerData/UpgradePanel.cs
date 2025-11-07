using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    private PlayerStatManager _playerStatManager;
    [SerializeField] private GameObject _popUpObject;
    [SerializeField] private TextMeshProUGUI _popUpText;
    [SerializeField] private UpgradeBtn upgradeBtnPrefab;
    [SerializeField] private GameObject ParentPanel;

    private void OnEnable()
    {

        EventBus.SetCanGetInput(false);
        if(_playerStatManager == null)
        {
        _playerStatManager = GameManager.Instance.PlayerStatManager;
        }

        if(upgradeBtnPrefab == null)
        {
            upgradeBtnPrefab = Resources.Load<UpgradeBtn>("Prefabs/Skill/UpgradeBtnPrefab");
        }
        foreach (Transform child in ParentPanel.transform)
        {
            Destroy(child.gameObject);
        }
        
        List<PlayerStatData> upgradeStats = _playerStatManager.GetUpgradableStats();

        if(upgradeStats.Count == 0)
        {
            Debug.LogWarning("업그레이드 가능 스탯 없음");
        }

        for(int i = 0; i< upgradeStats.Count; i++)
        {
         UpgradeBtn btn = Instantiate(upgradeBtnPrefab, ParentPanel.transform);
            btn.SetPlayerStatData(upgradeStats[i],this);
        }
    }



    public void OnClick_UpgradeBtn(UpgradeBtn btn)
    {
        string statName = btn.playerStatData.Column;

        if (!_playerStatManager.playerStatDataDic.ContainsKey(statName))
        {
            Debug.LogWarning("해당 스탯 이름이 없습니다.");
            return;
        }

        bool isLevelUp = _playerStatManager.playerStatDataDic[statName].LevelUp();
        Debug.Log($"[{statName}] 적용값: {_playerStatManager.playerStatDataDic[statName].GetEffectiveValue()}");

        btn.UpdatePanelData(); // UI 갱신
        PopUp(isLevelUp);
    }

    public void PopUp(bool isUpgrade)
    {
        _popUpObject.SetActive(true);

        string key = isUpgrade ? "UpgradeSuccess" : "UpgradeFail";
        _popUpText.text = GameManager.Instance.languageManager.GetLocalizedText(key);
    }

    private void OnDisable()
    {
        PlayerModel player = FindAnyObjectByType<PlayerModel>();
        player.Initialize();
        _playerStatManager.UploadToFirebase();
        
        EventBus.SetCanGetInput(true);
    }
}
