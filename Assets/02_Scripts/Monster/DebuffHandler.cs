using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DebuffHandler : MonoBehaviour
{
    public class DebuffData
    {
        public string debuffName;      // 디버프 이름
        public float debuffDuration;   // 지속시간
        public float debuffValue;      // 초당 대미지나 배수값
        public int maxStack;           // 최대 스택
    }

    private class ActiveDebuff
    {
        public DebuffData data;
        public float remainingTime;
        public float totalValue;
        public int stackCount;
        public Coroutine routine;

        public ActiveDebuff(DebuffData data, Coroutine routine)
        {
            this.data = data;
            remainingTime = data.debuffDuration;
            totalValue = data.debuffValue;
            stackCount = 1;
        }
    }

    private Dictionary<string, ActiveDebuff> activeDebuffs = new Dictionary<string, ActiveDebuff>();
    private float bonusDamage = 1f;
    private MonsterBase monster;
    private NavMeshAgent agent;

    private void Awake()
    {
        monster = GetComponent<MonsterBase>();
        agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// 디버프 적용
    /// </summary>
    public void ApplyDebuff(DebuffData newDebuff)
    {
        if (monster == null || monster.isDead)
            return;

        if (activeDebuffs.ContainsKey(newDebuff.debuffName))
        {
            ActiveDebuff existing = activeDebuffs[newDebuff.debuffName];

            // 스택형 디버프들
            if (newDebuff.debuffName == "독" || newDebuff.debuffName == "과부하")
            {
                if (existing.stackCount < newDebuff.maxStack)
                {
                    existing.stackCount++;
                    existing.totalValue += newDebuff.debuffValue;
                }
                existing.remainingTime = newDebuff.debuffDuration;
            }
            else
            {
                existing.remainingTime = newDebuff.debuffDuration;
                existing.totalValue = newDebuff.debuffValue;
            }

            return;
        }

        Coroutine routine = StartCoroutine(HandleDebuff(newDebuff));
        activeDebuffs.Add(newDebuff.debuffName, new ActiveDebuff(newDebuff, routine));
    }

    private IEnumerator HandleDebuff(DebuffData debuff)
    {
        ActiveDebuff active = activeDebuffs[debuff.debuffName];

        // 시작 시 1회 효과
        switch (debuff.debuffName)
        {
            case "둔화":
                if (agent != null)
                    agent.speed *= 0.7f;
                break;

            case "기절":
                StartCoroutine(StunCoroutine(debuff.debuffDuration));
                break;

            case "혼란":
                StartCoroutine(ConfuseCoroutine(debuff.debuffDuration));
                break;
        }

        while (active.remainingTime > 0f && !monster.isDead)
        {
            switch (debuff.debuffName)
            {
                case "독":
                case "과부하":
                    monster.TakeDamage(active.totalValue * bonusDamage); // 모든 피해 대미지 증가 이벤트 구독 필요
                    break;
            }

            active.remainingTime -= Time.deltaTime;
            yield return null;
        }

        // 해제 시 복원
        switch (debuff.debuffName)
        {
            case "둔화":
                if (agent != null)
                    agent.speed /= 0.7f;
                break;
        }

        activeDebuffs.Remove(debuff.debuffName);
    }

    private IEnumerator StunCoroutine(float duration)
    {
        monster.isStunned = true;

        if (agent != null)
            agent.isStopped = true;

        yield return new WaitForSeconds(duration);

        if (agent != null && !monster.isDead)
            agent.isStopped = false;

        monster.isStunned = false;
    }

    private IEnumerator ConfuseCoroutine(float duration)
    {
        string originalTag = monster.targetTag;
        monster.targetTag = "Monster";

        yield return new WaitForSeconds(duration);

        monster.targetTag = originalTag;
    }

    public bool CheckDebuff(DebuffData data)
    {
        return activeDebuffs.ContainsKey(data.debuffName);
    }

    public bool HasDebuff(string debuffName)
    {
        return activeDebuffs.ContainsKey(debuffName);
    }

    public int GetActiveDebuffCount()
    {
        int count = 0;

        foreach (KeyValuePair<string, ActiveDebuff> pair in activeDebuffs)
        {
            ActiveDebuff debuff = pair.Value;
            if (debuff != null && debuff.remainingTime > 0f)
                count++;
        }

        return count;
    }

    public int GetStackCount(string debuffName)
    {
        if (activeDebuffs.ContainsKey(debuffName))
            return activeDebuffs[debuffName].stackCount;
        return 0;
    }

    private void SetBonusDamage(float value)
    {
        bonusDamage = value;
    }
}
