using System.Collections.Generic;
using UnityEngine;

public class electricMan : MonoBehaviour
{
    [SerializeField] private int damage = 60;       // 충돌 시 데미지
    [SerializeField] private float damageInterval = 1f; // 같은 몬스터당 딜레이

    // 각 몬스터별 마지막 데미지 시간 저장
    private Dictionary<GameObject, float> lastDamageTime = new Dictionary<GameObject, float>();

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Monster")
        {
            GameObject monster = other.gameObject;
            float lastTime;
            // 딕셔너리에서 마지막 타이밍 가져오기 (없으면 기본값 0)
            lastDamageTime.TryGetValue(monster, out lastTime);
            if (Time.time - lastTime >= damageInterval)
            {
                // 실제 데미지 처리
                CharacterModelBase target = monster.GetComponent<CharacterModelBase>();
                if (target != null)
                {
                    target.TakeDamage(damage, null);
                    Debug.Log("전기인간 데미지 적용!");
                }
                // 타이머 갱신(데미지 적용 시간)
                lastDamageTime[monster] = Time.time;
            }
        }
    }
}
