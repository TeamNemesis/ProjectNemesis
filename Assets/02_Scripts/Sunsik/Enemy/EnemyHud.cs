using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 적 캐릭터의 HUD UI를 담당하는 역할
/// </summary>
public class EnemyHud : MonoBehaviour
{
    [SerializeField] Image _hpBar;

    public void SetHpBar(float currentHp, float maxHp)
    {
        _hpBar.fillAmount = currentHp / maxHp;
    }
}
