using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    HitEffect,
}

/// <summary>
/// 이펙트를 원하는 위치에 생성해 주는 클래스
/// </summary>
public class EffectSpawner : MonoBehaviour
{
    PoolManager _poolManager;

    private void Awake()
    {
        _poolManager = GameManager.Instance.PoolManager;
    }

    string GetPrefabPath(EffectType effectType)
    {
        return $"Effect/{effectType.ToString()}";
    }

    /// <summary>
    /// 이펙트를 생성하는 함수
    /// </summary>
    /// <param name="effectType">이펙트 종류</param>
    /// <param name="pos">위치</param>
    public void SpawnEffect(EffectType effectType, Vector3 pos)
    {
        GameObject effectGo = _poolManager.GetFromPool(GetPrefabPath(effectType));
        //effectGo.transform.SetParent(transform);
        effectGo.transform.position = pos;

        Effect effect = effectGo.GetComponent<Effect>();
        if (effect != null)
        {
            effect.Play();
        }
    }
}
